global using MesMicroservice.Domain.AggregateModels.OperationAggregate;
global using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
using MesMicroservice.Domain.AggregateModels.MaterialDefinitionAggregate;
using MesMicroservice.Domain.AggregateModels.ParameterAggregate;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace MesMicroservice.Algorithm.Services
{
    public class EquipmentSelectorService : IEquipmentSelectorService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IProductionParameterRepository _productionParameterRepository;
        private readonly ILogger<EquipmentSelectorService> _logger;

        public EquipmentSelectorService(IMaterialDefinitionRepository materialDefinitionRepository, IEquipmentRepository equipmentRepository, 
                                        IWorkOrderRepository workOrderRepository, ILogger<EquipmentSelectorService> logger,
                                        IProductionParameterRepository productionParameterRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
            _equipmentRepository = equipmentRepository;
            _workOrderRepository = workOrderRepository;
            _logger = logger;
            _productionParameterRepository = productionParameterRepository;
        }

        /// <summary>
        /// Helpers: assign eligible machines
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        public async Task AssignEligibleMachinesToJobsAsync(Dictionary<string, List<Job>> jobs)
        {

            foreach (var jobList in jobs.Values)
            {
                foreach (var job in jobList)
                {
                    var eligibleEquipments = await GetEligibleEquipmentsAsync(job.Id);

                    var machineList = new List<Machine>();

                    foreach (var equipment in eligibleEquipments)
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

                        var previousJob = null as Job;

                        if (availableTime > 0 && latestWorkOrder is not null)
                        {
                            var inputdiameter = 0.0;

                            foreach (var maSpec in latestWorkOrder.OperationsSegment.MaterialSpecifications)
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
                            var productionParameter = await _productionParameterRepository.GetAsync(latestWorkOrder.OperationsSegment.ProductionParameterId ?? "");
                            var properties = productionParameter?.Properties;
                            double GetDoubleProductionParameter(string name) =>
                                double.TryParse(properties?.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value?.ValueString,
                                    out var v) ? v : 0;
                            string EquipmentType = productionParameter?.Properties
                                .FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?
                                .Value?.ValueString ?? "";

                            var speed = GetDoubleProductionParameter("SPEED");
                            previousJob = WorkOrderMapper.ToJob(latestWorkOrder, inputdiameter, EquipmentType, speed);
                        }
                        
                        var machine = MachineMapper.FromEquipment(equipment, availableTime);
                        machine.PreviousJob = previousJob;
                        var type = job.Type;
                        var capability = equipment.EquipmentCapabilities?
                            .FirstOrDefault(c => c.Properties != null &&
                                                 c.Properties.Any(p => p.Name.Equals("type", StringComparison.OrdinalIgnoreCase) &&
                                                                      p.Value != null &&
                                                                      p.Value.ValueString.Equals(type, StringComparison.OrdinalIgnoreCase)));

                        if (capability is null)
                        {
                            if (!job.ConfiguredProcessingTimeHours.HasValue)
                                continue;

                            // Generic segments are selected by their equipment specifications.
                            // Material-specific capabilities are optional unless requested there.
                            machineList.Add(machine);
                            continue;
                        }
                        
                        double setupMin = capability?.Properties
                            .Where(p => p.Name.Equals("setup_time_min", StringComparison.OrdinalIgnoreCase))
                            .Select(p => double.TryParse(p.Value?.ValueString, out var v) ? v / 60 : 0)
                            .FirstOrDefault() ?? 0;

                        double setupMax = capability?.Properties
                            .Where(p => p.Name.Equals("setup_time_max", StringComparison.OrdinalIgnoreCase))
                            .Select(p => double.TryParse(p.Value?.ValueString, out var v) ? v / 60 : 0)
                            .FirstOrDefault() ?? 0;

                        var priorityValue = capability.Properties
                            .FirstOrDefault(p => p.Name.Equals("priority", StringComparison.OrdinalIgnoreCase))?
                            .Value?.ValueString;


                        machine.Properties.SetupTimeMin  = setupMin;
                        machine.Properties.SetupTimeMax  = setupMax;
                        machine.Properties.Priority = priorityValue != null && double.TryParse(priorityValue, out var p) ? p : 0;
                        machine.Properties.Speed = ConversionSpeed(job.SpeedRetirement);
                        machineList.Add(machine);
                    }

                    job.EligibleMachines = machineList
                        .DistinctBy(m => m.Code)
                        .ToList();
                }


            }
        }

        private double ConversionSpeed(double speed)
        {
            // Conversion speed from m/min to m/s
            return speed / 60.0;

        }

        /// <summary>
        /// Logic to get eligible equipments for a work order
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<List<Equipment>> GetEligibleEquipmentsAsync(int workOrderId)
        {
            var workOrder = await _workOrderRepository.GetById(workOrderId);

            var segment = workOrder.OperationsSegment
                ?? throw new InvalidOperationException("WorkOrder must have an associated OperationsSegment.");

            var requiredSpecs = segment.EquipmentSpecifications;
            var allEquipments = await _equipmentRepository.GetAllAsync();

            // Lọc máy trống
            allEquipments = allEquipments
    .Where(e => e.EquipmentStatus != EStatus.Pause
             && e.EquipmentStatus != EStatus.Breakdown
             && e.EquipmentStatus != EStatus.Maintenance)
    .ToList();

            var eligibleFinal = new List<Equipment>();
            foreach (var spec in requiredSpecs)
            {
                var matchingEquipments = allEquipments
                    .Where(e => e.EquipmentClass.ResourceId == spec.EquipmentClassId)
                    .ToList();

                var byDiameter = FilterByDiameter(spec, matchingEquipments);
                var byType = FilterByType(spec, matchingEquipments, byDiameter);
                var byLayer = FilterByEquipmentType(spec, byType);
                eligibleFinal.AddRange(byLayer);
                    
                //var byLayer = spec.EquipmentClassId == "EC02"
                //    ? FilterByLayerSupported(spec, matchingEquipments)
                //    : new List<Equipment>();
                    
                //eligibleFinal.AddRange(
                //    spec.EquipmentClassId == "EC02"
                //        ? byDiameter.Intersect(byType).Intersect(byLayer)
                //        : byDiameter.Intersect(byType)
                //);
            }

            var distinctEligible = eligibleFinal.DistinctBy(e => e.ResourceId).ToList();


            if (!distinctEligible.Any())
                _logger.LogWarning("No eligible equipments found for WorkOrder {WorkOrderId}", workOrder.WorkOrderId);

            return distinctEligible;
        }

        /// <summary>
        /// filter equipments by diameter
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="equipments"></param>
        /// <returns></returns>
        private static List<Equipment> FilterByDiameter(EquipmentSpecification spec, List<Equipment> equipments)
        {
            var result = new List<Equipment>();

            foreach (var equipment in equipments)
            {
                bool isEligible = true;

                foreach (var prop in spec.Properties)
                {
                    string name = prop.Name;



                    // ----- OUTPUT DIAMETER CHECK -----
                    if (name == "outputDiameter")
                    {
                        if (!TryParseDouble(prop.Value.ValueString, out double req))
                        {
                            isEligible = false;
                            break;
                        }
                        var minProp = equipment.Properties
                            .FirstOrDefault(p => p.Name.Equals("min-diameter", StringComparison.OrdinalIgnoreCase));
                        var maxProp = equipment.Properties
                            .FirstOrDefault(p => p.Name.Equals("max-diameter", StringComparison.OrdinalIgnoreCase));

                        if (minProp is null || maxProp is null ||
                            !TryParseDouble(minProp.Value.ValueString, out double min) ||
                            !TryParseDouble(maxProp.Value.ValueString, out double max) ||
                            req < min || req > max)
                        {
                            isEligible = false;
                            break;
                        }
                    }

                    // ----- INPUT DIAMETER CHECK -----
                    if (name == "inputDiameter")
                    {
                        if (!TryParseDouble(prop.Value.ValueString, out double req))
                        {
                            isEligible = false;
                            break;
                        }
                        var inputMinProp = equipment.Properties
                            .FirstOrDefault(p => p.Name.Equals("inputMin", StringComparison.OrdinalIgnoreCase));
                        var inputMaxProp = equipment.Properties
                            .FirstOrDefault(p => p.Name.Equals("inputMax", StringComparison.OrdinalIgnoreCase));

                        if (inputMinProp is null || inputMaxProp is null ||
                            !TryParseDouble(inputMinProp.Value.ValueString, out double inMin) ||
                            !TryParseDouble(inputMaxProp.Value.ValueString, out double inMax) ||
                            req < inMin || req > inMax)
                        {
                            isEligible = false;
                            break;
                        }
                    }
                }

                if (isEligible)
                    result.Add(equipment);
            }

            return result;
        }


        /// <summary>
        /// filter equipments by type
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="equipments"></param>
        /// <param name="filteredByDiameter"></param>
        /// <returns></returns>
        private static List<Equipment> FilterByType(
            EquipmentSpecification spec,
            List<Equipment> equipments,
            List<Equipment> filteredByDiameter)
        {
            if (spec.Properties == null || !spec.Properties.Any())
                return filteredByDiameter;

            var typeSpec = spec.Properties
                .FirstOrDefault(p => p.Name.Equals("Type", StringComparison.OrdinalIgnoreCase));

            if (typeSpec is null)
                return filteredByDiameter;

            var result = equipments
                .Select(e =>
                {
                    // Lọc capability theo Type
                    var matchingCapabilities = e.EquipmentCapabilities?
                        .Where(cap =>
                            cap.Properties?.Any(prop =>
                                prop.Name.Equals("type", StringComparison.OrdinalIgnoreCase) &&
                                prop.Value?.ValueString?.Equals(
                                    typeSpec.Value.ValueString,
                                    StringComparison.OrdinalIgnoreCase) == true
                            ) == true
                        )
                        .ToList();

                    if (matchingCapabilities == null || !matchingCapabilities.Any())
                        return null;

                    // Clone hoặc gán lại capability đã lọc
                    e.EquipmentCapabilities = matchingCapabilities;
                    return e;
                })
                .Where(e => e != null)
                .Intersect(filteredByDiameter)
                .ToList();

            return result;
        }


        /// <summary>
        /// Filter equipments by equipmentType
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="equipments"></param>
        /// <returns></returns>
        private static List<Equipment> FilterByEquipmentType(EquipmentSpecification spec, List<Equipment> equipments)
        {
            var result = new List<Equipment>();

            // Lấy type cần lọc, ví dụ "KD"
            var typeFilter = spec.Properties
                .FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))
                ?.Value.ValueString ?? "";

            if (string.IsNullOrWhiteSpace(typeFilter))
                return equipments;

            // Lọc đúng theo prefix của mã máy (ResourceId)
            foreach (var equipment in equipments)
            {
                string equipmentId = equipment.ResourceId ?? "";

                // Máy hợp lệ khi bắt đầu bằng đúng type
                if (equipmentId.StartsWith(typeFilter, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(equipment);
                }
            }

            return result;
        }


        /// <summary>
        /// filter equipments by layerSupported
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="equipments"></param>
        /// <returns></returns>
        private static List<Equipment> FilterByLayerSupported(EquipmentSpecification spec, List<Equipment> equipments)
        {
            var result = new List<Equipment>();

            bool Compare(Func<int, int, bool> condition)
            {
                foreach (var equipment in equipments)
                {
                    foreach (var prop in spec.Properties)
                    {
                        if (!prop.Name.Equals("layerSupported", StringComparison.OrdinalIgnoreCase)) continue;

                        var equipProp = equipment.Properties
                            .FirstOrDefault(p => p.Name.Equals("layerSupported", StringComparison.OrdinalIgnoreCase));
                        if (equipProp is null) continue;

                        if (TryParseInt(prop.Value.ValueString, out int req)
                            && TryParseInt(equipProp.Value.ValueString, out int val)
                            && condition(val, req))
                        {
                            result.Add(equipment);
                        }
                    }
                }
                return result.Any();
            }

            // Ưu tiên: bằng nhau → lớn hơn → nhỏ hơn
            if (!Compare((v, r) => v == r))
                if (!Compare((v, r) => v > r))
                    Compare((v, r) => v < r);

            return result;
        }

        /// <summary>
        /// Helper: try parse double with invariant culture
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParseDouble(string s, out double result) =>
            double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

        private static bool TryParseInt(string s, out int result) =>
            int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }
}
