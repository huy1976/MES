using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MesMicroservice.Algorithm.Models;
using MesMicroservice.Algorithm.Models.Mapping;
using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
using MesMicroservice.Algorithm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MesMicroservice.Application.Interfaces;

namespace MesMicroservice.Application.Commands.TabuData
{
    //public class GenerateTabuDataCommandHandler : IRequestHandler<GenerateTabuDataCommand, Solution>
    //{
    //    private readonly IWorkOrderRepository _workOrderRepo;
    //    private readonly IEquipmentRepository _equipmentRepo;
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly IEquipmentSelectorService _equipmentSelectorService;
    //    private readonly IProductionParameterRepository _productionParameterRepo;
    //    private readonly IMaterialDefinitionRepository _materialDefinitionRepo;
    //    //private readonly TabuAIPredictor _tabuAIPredictor;

    //    public GenerateTabuDataCommandHandler(
    //        IWorkOrderRepository workOrderRepo,
    //        IEquipmentRepository equipmentRepo,
    //        IServiceProvider serviceProvider,
    //        IEquipmentSelectorService equipmentSelectorService,
    //        IProductionParameterRepository productionParameterRepo,
    //        IMaterialDefinitionRepository materialDefinitionRepo
    //        //TabuAIPredictor tabuAIPredictor
    //        )
    //    {
    //        _workOrderRepo = workOrderRepo;
    //        _equipmentRepo = equipmentRepo;
    //        _serviceProvider = serviceProvider;
    //        _equipmentSelectorService = equipmentSelectorService;
    //        _productionParameterRepo = productionParameterRepo;
    //        _materialDefinitionRepo = materialDefinitionRepo;
    //        //_tabuAIPredictor = tabuAIPredictor;
    //    }

//        public async Task<Solution> Handle(GenerateTabuDataCommand request, CancellationToken cancellationToken)
//        {
//            // 1. LẤY DỮ LIỆU TỪ DATABASE
//            var workOrders = await _workOrderRepo.GetByWorkOrderCodesAsync(request.WorkOrderIds);
//            var equipments = await _equipmentRepo.GetAllAsync();

//            if (workOrders == null || !workOrders.Any())
//                throw new Exception("Không tìm thấy dữ liệu WorkOrder.");

//            // 2. MAPPING SANG JOB (DÙNG MAPPER CHUẨN)
//            List<Job> jobs = new List<Job>();
//            foreach (var wo in workOrders)
//            {
//                var prodParam = await _productionParameterRepo.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
//                var prodProps = prodParam?.Properties;
//                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
//                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

//                double inputDiam = 0;
//                if (wo.OperationsSegment?.MaterialSpecifications != null)
//                {
//                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
//                    {
//                        var matDef = await _materialDefinitionRepo.GetAsync(maSpec.MaterialDefinitionId ?? "");
//                        if (matDef != null)
//                        {
//                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
//                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0) { inputDiam = d; break; }
//                        }
//                    }
//                }

//                // Gọi Mapper gốc để đảm bảo tính đúng đắn của logic nghiệp vụ
//                var job = WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed);
//                jobs.Add(job);
//            }

//            // 3. TẠO DANH SÁCH MÁY TỔNG (DÙNG ResourceId LÀM KHÓA)
//            List<Machine> machines = equipments.Select(eq => new Machine
//            {
//                Id = eq.Id,
//                // ÉP BUỘC: Dùng ResourceId (VD: "MD05") làm Code để khớp với Service chọn máy
//                Code = eq.ResourceId ?? eq.Id.ToString()
//            }).ToList();

//            var jobDict = jobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());

//            // Gán máy hợp lệ (Service này thường trả về List máy có Code = ResourceId)
//            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobDict);

//            // --- BƯỚC ĐỒNG BỘ HÓA TUYỆT ĐỐI (FIX LỖI 'm was null') ---
//            foreach (var job in jobs)
//            {
//                if (job.EligibleMachines == null || !job.EligibleMachines.Any())
//                    throw new Exception($"Lệnh {job.Name} không tìm thấy máy phù hợp cho loại '{job.EquipmentType}'.");

//                // Đảm bảo mọi máy trong EligibleMachines đều có mã Code tồn tại trong danh sách machines tổng
//                foreach (var em in job.EligibleMachines)
//                {
//                    // Nếu Service gán máy dùng ResourceId, mà machines tổng dùng Id (hoặc ngược lại)
//                    // Ta phải tìm máy tương ứng trong danh sách equipments để lấy đúng ResourceId
//                    var masterEquipment = equipments.FirstOrDefault(e => e.ResourceId == em.Code || e.Id.ToString() == em.Code);
//                    if (masterEquipment != null)
//                    {
//                        em.Code = masterEquipment.ResourceId ?? masterEquipment.Id.ToString();
//                    }
//                }
//            }

//            // 4. CHẠY THUẬT TOÁN
//            var initialSchedule = new Schedule(machines, jobDict);
//            var insertionMethod = _serviceProvider.GetRequiredService<InsertionMethod>();
//            var baseSolution = await insertionMethod.Run(initialSchedule);

//            var tabuService = _serviceProvider.GetRequiredService<TabuSearchService>();

//            // =========================================================================
//            // ĐÃ SỬA LỖI Ở ĐÂY: Dùng ngoặc tròn để hứng trực tiếp giá trị từ hàm AI
//            // =========================================================================
//            int copperJobsCount = jobs.Count(j =>
//    !string.IsNullOrEmpty(j.Name) &&
//    (j.Name.Contains("Cu", StringComparison.Ordinal) || j.Name.Contains("CU", StringComparison.Ordinal) ||
//     j.Name.Contains("Đồng", StringComparison.OrdinalIgnoreCase) || j.Name.Contains("Dong", StringComparison.OrdinalIgnoreCase))
//);

//            int aluminumJobsCount = jobs.Count(j =>
//                !string.IsNullOrEmpty(j.Name) &&
//                (j.Name.Contains("Al", StringComparison.Ordinal) || j.Name.Contains("AL", StringComparison.Ordinal) ||
//                 j.Name.Contains("Nhôm", StringComparison.OrdinalIgnoreCase) || j.Name.Contains("Nhom", StringComparison.OrdinalIgnoreCase))
//            );

//            // ====================================================
//            // 2. GỌI AI DỰ ĐOÁN THAM SỐ (Truyền đúng 3 tham số kiểu int)
//            // ====================================================
//            //var (optimalTenure, optimalNeighbors) = _tabuAIPredictor.PredictOptimalParameters(
//            //    jobs.Count,           // Tham số 1: Tổng số lệnh
//            //    copperJobsCount,      // Tham số 2: Số lệnh Đồng 
//            //    aluminumJobsCount,    // Tham số 3: Số lệnh Nhôm 
//            //    machines.Count        // Tham số 4: Số lượng máy thực tế
//            //);

//            // Cập nhật lại câu lệnh in ra Console 
//            //Console.WriteLine($"[AI QUYẾT ĐỊNH]: Dành cho {jobs.Count} Lệnh ({machines.Count} máy) -> Tenure={optimalTenure}, Neighbors={optimalNeighbors}");

//            //var tabuSettings = new TabuSearchSettings
//            //{
//            //    // Cập nhật lại gán biến cho Settings
//            //    TabuTenure = optimalTenure,
//            //    NumNeighborsPerMoAndStage = optimalNeighbors,
//            //    MaxIterations = 500
//            //};

//            //var finalSolution = await tabuService.RunWithSettings(baseSolution.BestSchedule, tabuSettings);

//            // 5. LƯU DỮ LIỆU
//            //ExportTrainingData(jobs, machines, request.TabuTenure, request.NumNeighbors, finalSolution);

//            //return finalSolution;
//        }

        //private void ExportTrainingData(List<Job> jobs, List<Machine> machines, int tenure, int neighbors, Solution solution)
        //{
        //    string csvLine = $"{jobs.Count},{machines.Count},{jobs.Sum(j => j.ProcessingTime)},{tenure},{neighbors},{solution.BestSchedule.Makespan}";
        //    try { System.IO.File.AppendAllText("Tabu_Train_Data.csv", csvLine + Environment.NewLine); } catch { }
        //}
    //}
}