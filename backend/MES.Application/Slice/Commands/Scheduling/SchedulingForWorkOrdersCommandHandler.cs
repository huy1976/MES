global using MesMicroservice.Algorithm.Models.Mapping;
using Microsoft.Identity.Client;

namespace MesMicroservice.Application.Commands.Scheduling
{
    public class SchedulingForWorkOrdersCommandHandler : IRequestHandler<SchedulingForWorkOrdersCommand, Solution?>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly ISchedulingService _schedulingService;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IPersonnelSelectorService _personnelSelectorService;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IProductionParameterRepository _productionParameterRepository;
        private readonly IMapper _mapper;

        public SchedulingForWorkOrdersCommandHandler(IWorkOrderRepository workOrderRepository, ISchedulingService schedulingService, 
                                                     IEquipmentRepository equipmentRepository, IPersonnelSelectorService personnelSelectorService, 
                                                     IMapper mapper, IMaterialDefinitionRepository materialDefinitionRepository,
                                                     IProductionParameterRepository productionParameterRepository)
        {
            _workOrderRepository = workOrderRepository;
            _schedulingService = schedulingService;
            _equipmentRepository = equipmentRepository;
            _personnelSelectorService = personnelSelectorService;
            _mapper = mapper;
            _materialDefinitionRepository = materialDefinitionRepository;
            _productionParameterRepository = productionParameterRepository;

        }

        public async Task<Solution?> Handle(SchedulingForWorkOrdersCommand request, CancellationToken cancellationToken)
        {
            var workOrders = await _workOrderRepository.GetByIds(request.WorkOrderIds);
            var equipments = await _equipmentRepository.GetAllAsync();

            equipments = equipments.Where(e => e.EquipmentStatus != EStatus.Breakdown && e.EquipmentStatus != EStatus.Maintenance).ToList();

            var jobs = new List<Job>();
            var urgentWorkOrderIds = request.UrgentWorkOrderIds?.ToHashSet() ?? new HashSet<int>();
            foreach (var wo in workOrders)
            {
                var inputdiameter = 0.0;

                foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                {
                    
                    var materialDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                    if (materialDef is not null)
                    {
                        var props = materialDef.Properties;
                        double GetDouble(string name) =>
                        double.TryParse(props.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString,
                            out var v) ? v : 0;
                        inputdiameter = GetDouble("K/C");
                        if (inputdiameter > 0.0)
                        {
                            break;
                        }

                    }

                }

                var productionParameter = await _productionParameterRepository.GetAsync(wo.OperationsSegment.ProductionParameterId ?? "");
                var properties = productionParameter?.Properties;
                string type = productionParameter?.Properties
                    .FirstOrDefault(p => p.Name != null && p.Name.Trim().Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?
                    .Value?.ValueString ?? "";
                double GetDoubleProductionParameter(string name)
                {
                    if (properties == null || !properties.Any()) return 0;

                    // Tìm kiếm an toàn: Bọc điều kiện p.Name != null để chống Crash
                    var prop = properties.FirstOrDefault(p =>
                        p.Name != null && p.Name.Trim().Equals(name, StringComparison.OrdinalIgnoreCase));

                    // Nếu không tìm thấy hoặc giá trị bị rỗng
                    if (prop == null || prop.Value == null || string.IsNullOrWhiteSpace(prop.Value.ValueString))
                    {
                        return 0;
                    }

                    // Ép kiểu chuẩn Quốc tế (InvariantCulture) để không bị lỗi dấu phẩy/chấm
                    string cleanValue = prop.Value.ValueString.Trim().Replace(",", ".");

                    if (double.TryParse(cleanValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
                    {
                        return result;
                    }

                    return 0;
                }

                var speed = GetDoubleProductionParameter("SPEED");

                var job = WorkOrderMapper.ToJob(wo, inputdiameter, type, speed);
                if (urgentWorkOrderIds.Contains(wo.Id))
                {
                    job.Priority = 100;
                }
                jobs.Add(job);
            }

            var machineList = new List<Machine>();

            foreach (var equipment in equipments)
            {
                var assignedWorkOrders = await _workOrderRepository
                    .GetByAssignedEquipmentId(equipment.ResourceId);

                var latestWorkOrder = assignedWorkOrders
                    .SelectMany(wo => wo.EquipmentWorkOrders)
                    .Where(eo => eo.Equipment.ResourceId == equipment.ResourceId)
                    .Select(eo => eo.WorkOrder)
                    .Where(wo => wo.EndTime != null)
                    .OrderByDescending(wo => wo.EndTime)               // ① Ưu tiên lệnh phía sau
                    .ThenByDescending(wo => wo.ActuallyEndTime)        // ② Nếu cùng EndTime thì ưu tiên actual
                    .FirstOrDefault();


                DateTime? latestEndTime = latestWorkOrder?.EndTime;
                DateTime? actualLatestEndTime = latestWorkOrder?.ActuallyEndTime;

                DateTime? endTimeToConsider =
                    (actualLatestEndTime.HasValue && actualLatestEndTime.Value != DateTime.MinValue)
                        ? actualLatestEndTime
                        : latestEndTime;


                double availableTime = (endTimeToConsider > DateTime.Now)
                    ? (endTimeToConsider.Value - DateTime.Now).TotalHours
                    : 0;

                machineList.Add(MachineMapper.FromEquipment(equipment, availableTime));
            }

            var solution = await _schedulingService.GenerateScheduleAsync(jobs, machineList, request.AlgorithmType.ToString(), request.UseAI);

            // Chỉ giữ lại máy cần thiết
            solution.BestSchedule.Machines = solution.BestSchedule.Machines
                .Where(m => m.TotalTime > 0)
                .ToList();

            // xóa cababilies cho ngắn lại
            foreach (var machine in solution.BestSchedule.Machines)
            {
                machine.Capabilities = new();
            }

            // xóa cababilies trong eligibleMachines
            foreach (var moJobs in solution.BestSchedule.Jobs)
            {
                foreach (var job in moJobs.Value)
                {
                    foreach (var em in job.EligibleMachines)
                    {
                        em.Capabilities = new();
                    }
                }
            }

            solution.BestSchedule = await _personnelSelectorService.SelectPersonnel(solution.BestSchedule);

            return solution;


        }
    }
}


//foreach (var machineJobs in solution.BestSchedule.MachineJobs)
//{
//    double currentTime = 0;


//    var equipmentId = machineJobs.Key.Id;
//    var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);

//    foreach (var job in machineJobs.Value)
//    {
//        var workOrder = workOrders.First(w => w.WorkOrderId == job.Id);

//        var start = DateTime.Now.AddMinutes(currentTime);
//        var end = DateTime.Now.AddMinutes(currentTime + job.ProcessingTime);

//        workOrder.UpdateScheduleTimes(start, end);

//        workOrder.EquipmentWorkOrders.Clear();
//        workOrder.EquipmentWorkOrders.Add(new EquipmentWorkOrder(equipment, equipment.ClassName, workOrder));


//        workOrder.SetStatus(EStatus.Scheduled);

//        currentTime += job.ProcessingTime;
//    }
//}


//await _workOrderRepository.UpdateRangeAsync(workOrders);
//await _unitOfWork.SaveChangesAsync(cancellationToken);


//return solution.BestSchedule.MachineJobs
//    .SelectMany(mj => mj.Value.Select(job => new ScheduledWorkOrderDTO
//    {
//        WorkOrderId = job.Id,
//        MachineId = mj.Key.Id,
//        StartTime = DateTime.Now.AddMinutes(job.StartTime),
//        EndTime = DateTime.Now.AddMinutes(job.EndTime)
//    }))
//    .ToList();