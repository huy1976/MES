using MediatR;
using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
using MesMicroservice.Algorithm.Interfaces;
// Các namespace chứa thuật toán và model
using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
using MesMicroservice.Algorithm.Models;
using MesMicroservice.Algorithm.Models.Enum;
using MesMicroservice.Algorithm.Models.Mapping; // Để dùng WorkOrderMapper và MachineMapper
using MesMicroservice.Algorithm.Services;
using MesMicroservice.Application.Commands;
using MesMicroservice.Application.Commands.TabuData;
using MesMicroservice.MachineLearning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MLDataGeneratorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TabuAIPredictor _tabuAIPredictor;
        private readonly TabuSearchService _tabuSearchService;

        // Các Repository dùng để lấy dữ liệu thật
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;
        private readonly IProductionParameterRepository _productionParameterRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEquipmentSelectorService _equipmentSelectorService;

        // Bơm (Inject) tất cả các service vào Constructor
        public MLDataGeneratorController(
            IMediator mediator,
            TabuAIPredictor tabuAIPredictor,
            TabuSearchService tabuSearchService,
            IWorkOrderRepository workOrderRepository,
            IEquipmentRepository equipmentRepository,
            IMaterialDefinitionRepository materialDefinitionRepository,
            IProductionParameterRepository productionParameterRepository,
            IServiceProvider serviceProvider,
            IEquipmentSelectorService equipmentSelectorService)
        {
            _mediator = mediator;
            _tabuAIPredictor = tabuAIPredictor;
            _tabuSearchService = tabuSearchService;
            _workOrderRepository = workOrderRepository;
            _equipmentRepository = equipmentRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _productionParameterRepository = productionParameterRepository;
            _serviceProvider = serviceProvider;
            _equipmentSelectorService = equipmentSelectorService;
        }



        [HttpPost("automate-loop-generation")]
        public async Task<IActionResult> AutomateLoop([FromBody] AutomateTabuDataCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("test-ai-prediction")]
        public IActionResult TestAIPrediction([FromBody] ScheduleRequest request)
        {
            // 1. Lấy tổng số lượng Lệnh (check null an toàn)
            int totalJobs = request.Jobs?.Count ?? 0;

            // 2. Đếm số lệnh Đồng 
            int copperJobsCount = request.Jobs?.Count(j =>
                !string.IsNullOrEmpty(j.Name) &&
                (j.Name.Contains("Cu", StringComparison.Ordinal) || j.Name.Contains("CU", StringComparison.Ordinal) ||
                 j.Name.Contains("Đồng", StringComparison.OrdinalIgnoreCase) || j.Name.Contains("Dong", StringComparison.OrdinalIgnoreCase))
            ) ?? 0;

            // 3. Đếm số lệnh Nhôm
            int aluminumJobsCount = request.Jobs?.Count(j =>
                !string.IsNullOrEmpty(j.Name) &&
                (j.Name.Contains("Al", StringComparison.Ordinal) || j.Name.Contains("AL", StringComparison.Ordinal) ||
                 j.Name.Contains("Nhôm", StringComparison.OrdinalIgnoreCase) || j.Name.Contains("Nhom", StringComparison.OrdinalIgnoreCase))
            ) ?? 0;

            // 4. Gọi hàm AI dự đoán Version
            int machineCount = 12;
            var features = new TabuAITrainingData
            {
                TotalJobs = totalJobs,
                MachineCount = machineCount,

                AvgEligibleMachines = 1,
                MinEligibleMachines = 1,
                MaxEligibleMachines = 1,

                AvgProcessingTime = 0,
                StdProcessingTime = 0,

                AvgDiameter = 0,
                DiameterVariance = 0,

                AvgSetupTime = 0,
                StdSetupTime = 0,

                SetupConflictRatio = 0,
                SameMaterialRatio = 0,
                SameDiameterRatio = 0,

                MachineSpeedStd = 0,
                MachineEfficiencyStd = 0,
                MachineCapabilityImbalance = 0,

                AssignmentSensitivity = 0,
                MachinePreferenceSharpness = 0,

                BottleneckLoadRatio = 0,
                MachineLoadStd = 0,
                BottleneckSensitivity = 0,

                TightDueDateRatio = 0,
                AvgSlackTime = 0,
                CriticalJobRatio = 0,

                InitialMakespan = 0,
                InitialObjective = 0
            };

            var (optimalTenure, optimalNeighbors) =
                _tabuAIPredictor.PredictBestParameters(features);
            return Ok(new
            {
                Message = "AI Model Version 2.0 is running perfectly!",
                Input = new
                {
                    TotalJobs = totalJobs,
                    CopperJobs = copperJobsCount,
                    AluminumJobs = aluminumJobsCount
                },
                AiRecommendation = new
                {
                    Tenure = optimalTenure,
                    Neighbors = optimalNeighbors
                }
            });
        }
        [HttpPost("run-benchmark")]
        public async Task<IActionResult> RunBenchmark([FromBody] List<string> workOrderIds)
        {
            if (workOrderIds == null || workOrderIds.Count < 2)
                return BadRequest("Vui lòng truyền vào ít nhất 2 ID lệnh sản xuất.");

            var allWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(workOrderIds);
            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments
                .Where(e => e.EquipmentStatus != EStatus.Breakdown
                         && e.EquipmentStatus != EStatus.Maintenance
                         && e.EquipmentStatus != EStatus.Pause)
                .ToList();

            if (allWorkOrders == null || !allWorkOrders.Any())
                return BadRequest("Không tìm thấy dữ liệu WorkOrder.");

            var objectiveEvaluator = new ObjectiveEvaluator();
            List<Job> allJobs = new List<Job>();

            foreach (var wo in allWorkOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;
                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0)
                            {
                                inputDiam = d;
                                break;
                            }
                        }
                    }
                }

                allJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            List<Machine> machines = equipments.Select(eq => new Machine
            {
                Id = eq.Id,
                Code = eq.ResourceId ?? eq.Id.ToString()
            }).ToList();

            var allJobDict = allJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());
            await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(allJobDict);

            foreach (var job in allJobs)
            {
                if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                    continue;

                foreach (var em in job.EligibleMachines)
                {
                    var masterEq = equipments.FirstOrDefault(e =>
    (e.ResourceId != null && e.ResourceId == em.Code) ||
    (e.Id != null && e.Id.ToString() == em.Code));
                    if (masterEq != null)
                        em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                }
            }

            string logFilePath = "Benchmark_AI_vs_Fixed.csv";
            if (!System.IO.File.Exists(logFilePath))
            {
                await System.IO.File.WriteAllTextAsync(
                    logFilePath,
                    "Timestamp,TotalJobs,CopperJobs,AluminumJobs,MachineCount,Fixed_Tenure,Fixed_Neighbors,Fixed_Runtime_ms,Fixed_Objective,AI_Tenure,AI_Neighbors,AI_Runtime_ms,AI_Objective\n",
                    Encoding.UTF8);
            }

            var summaryResults = new System.Collections.Concurrent.ConcurrentBag<object>();
            object fileLock = new object();

            int maxThreads = Math.Max(1, Environment.ProcessorCount - 1);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            var jobCountsToRun = Enumerable.Range(2, Math.Max(0, allJobs.Count - 1)).ToList();

            Console.WriteLine($"\nRUNNING");
            Console.WriteLine($"[Benchmark] Total Jobs = {allJobs.Count}");
            Console.WriteLine($"[Benchmark] Scenarios = {string.Join(", ", jobCountsToRun)}");

            await Parallel.ForEachAsync(jobCountsToRun, parallelOptions, async (currentJobCount, cancellationToken) =>
            {
                using var scope = _serviceProvider.CreateScope();

                var insertionMethod = scope.ServiceProvider.GetRequiredService<InsertionMethod>();
                var tabuService = scope.ServiceProvider.GetRequiredService<TabuSearchService>();

                var originalJobs = allJobs
                    .Take(currentJobCount)
                    .Select(j => j.Clone())
                    .ToList();

                if (originalJobs.Count < 2)
                    return;

                if (originalJobs.Any(j => j.EligibleMachines == null || !j.EligibleMachines.Any()))
                {
                    Console.WriteLine($"[SKIP] {currentJobCount} Jobs - thiếu EligibleMachines.");
                    return;
                }

                int copperJobsCount = originalJobs.Count(j =>
                    !string.IsNullOrEmpty(j.Name) &&
                    (j.Name.Contains("Cu", StringComparison.Ordinal) ||
                     j.Name.Contains("CU", StringComparison.Ordinal) ||
                     j.Name.Contains("Đồng", StringComparison.OrdinalIgnoreCase) ||
                     j.Name.Contains("Dong", StringComparison.OrdinalIgnoreCase)));

                int aluminumJobsCount = originalJobs.Count(j =>
                    !string.IsNullOrEmpty(j.Name) &&
                    (j.Name.Contains("Al", StringComparison.Ordinal) ||
                     j.Name.Contains("AL", StringComparison.Ordinal) ||
                     j.Name.Contains("Nhôm", StringComparison.OrdinalIgnoreCase) ||
                     j.Name.Contains("Nhom", StringComparison.OrdinalIgnoreCase)));

                var activeMachineCodes = originalJobs
                    .Where(j => j.EligibleMachines != null)
                    .SelectMany(j => j.EligibleMachines)
                    .Select(em => em.Code)
                    .Distinct()
                    .ToList();

                int realMachineCount = activeMachineCodes.Count > 0 ? activeMachineCodes.Count : 12;

                var sharedInitialSchedule = new MesMicroservice.Algorithm.Models.Schedule(
                    machines.Select(m => m.Clone()).ToList(),
                    originalJobs
                        .GroupBy(j => j.ManufacturingOrderId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(j => j.Clone()).ToList())
                );

                var baseSolution = await insertionMethod.Run(sharedInitialSchedule);
                var baseScheduleForMetrics = baseSolution.BestSchedule.Clone();
                var baseObjective = objectiveEvaluator.Evaluate(baseScheduleForMetrics);

                var fixedInitial = baseSolution.BestSchedule.Clone();
                var aiInitial = baseSolution.BestSchedule.Clone();

                const int FIXED_TENURE = 10;
                const int FIXED_NEIGHBORS = 20;

                int scenarioSeed = CreateScenarioSeed(currentJobCount, workOrderIds);

                var fixedSettings = new TabuSearchSettings
                {
                    UseAI = false,
                    TabuTenure = FIXED_TENURE,
                    NumNeighborsPerMoAndStage = FIXED_NEIGHBORS,
                    MaxIterations = 1500,
                    MaxIdleIterations = 1000
                };

                // 1. Khởi tạo Features ban đầu (Giữ các giá trị cơ bản, các tỷ lệ phức tạp để 0 để tính sau)
                var features = new TabuAITrainingData
                {
                    TotalJobs = currentJobCount,
                    MachineCount = realMachineCount,

                    AvgEligibleMachines = originalJobs.Count > 0
                        ? (float)originalJobs.Average(j => j.EligibleMachines?.Count ?? 1)
                        : 1,

                    MinEligibleMachines = originalJobs.Count > 0
                        ? originalJobs.Min(j => j.EligibleMachines?.Count ?? 1)
                        : 1,

                    MaxEligibleMachines = originalJobs.Count > 0
                        ? originalJobs.Max(j => j.EligibleMachines?.Count ?? 1)
                        : 1,

                    AvgProcessingTime = originalJobs.Count > 0
                        ? (float)originalJobs.Average(j => j.ProcessingTime)
                        : 0,

                    StdProcessingTime = originalJobs.Count > 1
                        ? (float)Math.Sqrt(
                            originalJobs.Average(j => Math.Pow(j.ProcessingTime - originalJobs.Average(x => x.ProcessingTime), 2)))
                        : 0,

                    AvgDiameter = originalJobs.Count > 0
                        ? (float)originalJobs.Average(j => j.Diameter)
                        : 0,

                    DiameterVariance = originalJobs.Count > 1
                        ? (float)originalJobs.Average(j => Math.Pow(j.Diameter - originalJobs.Average(x => x.Diameter), 2))
                        : 0,

                    AvgSetupTime = originalJobs.Count > 0
                        ? (float)originalJobs.Average(j => j.SetupTime)
                        : 0,

                    StdSetupTime = originalJobs.Count > 1
                        ? (float)Math.Sqrt(
                            originalJobs.Average(j => Math.Pow(j.SetupTime - originalJobs.Average(x => x.SetupTime), 2)))
                        : 0,

                    // ĐỂ TẠM BẰNG 0, LÁT NỮA TÍNH XONG SẼ GÁN LẠI
                    SetupConflictRatio = 0,
                    SameMaterialRatio = 0,
                    SameDiameterRatio = 0,
                    MachineSpeedStd = 0,
                    MachineEfficiencyStd = 0,
                    MachineCapabilityImbalance = 0,
                    AssignmentSensitivity = 0,
                    MachinePreferenceSharpness = 0,
                    BottleneckLoadRatio = 0,
                    MachineLoadStd = 0,
                    BottleneckSensitivity = 0,
                    TightDueDateRatio = 0,
                    AvgSlackTime = 0,
                    CriticalJobRatio = 0,

                    InitialMakespan = (float)objectiveEvaluator.CalculateMakespan(baseScheduleForMetrics),
                    InitialObjective = (float)baseObjective
                };

                // LƯU Ý: Ở BẢN CODE CŨ, BẠN GỌI AI DỰ ĐOÁN NGAY TẠI ĐÂY LÀ SAI NHÉ!
                // MÌNH ĐÃ XÓA ĐI RỒI!

                // ====================================================================
                // 2. CHẠY VÒNG LẶP ĐỂ TÍNH TOÁN CÁC HỆ SỐ CÒN THIẾU
                // ====================================================================
                int setupConflict = 0;
                int sameMaterial = 0;
                int sameDiameter = 0;

                for (int i = 0; i < originalJobs.Count - 1; i++)
                {
                    var a = originalJobs[i];
                    var b = originalJobs[i + 1];

                    if (a.MaterialDefinitionId != b.MaterialDefinitionId)
                        setupConflict++;
                    else
                        sameMaterial++;

                    if (Math.Abs(a.Diameter - b.Diameter) < 0.01)
                        sameDiameter++;
                }

                int transitions = Math.Max(1, originalJobs.Count - 1);

                // BẮT ĐẦU CẬP NHẬT CÁC HỆ SỐ VÀO BIẾN features
                features.SetupConflictRatio = (float)setupConflict / transitions;
                features.SameMaterialRatio = (float)sameMaterial / transitions;
                features.SameDiameterRatio = (float)sameDiameter / transitions;

                var machineSpeeds = sharedInitialSchedule.Machines?
                    .Select(m => m.Properties.Speed)
                    .ToList() ?? new List<double>();

                if (machineSpeeds.Count > 1)
                {
                    double avgSpeed = machineSpeeds.Average();
                    features.MachineSpeedStd = (float)Math.Sqrt(machineSpeeds.Average(x => Math.Pow(x - avgSpeed, 2)));
                }

                var machineEff = sharedInitialSchedule.Machines?
                    .Select(m => m.Properties.Efficiency)
                    .ToList() ?? new List<double>();

                if (machineEff.Count > 1)
                {
                    double avgEff = machineEff.Average();
                    features.MachineEfficiencyStd = (float)Math.Sqrt(machineEff.Average(x => Math.Pow(x - avgEff, 2)));
                }

                features.AssignmentSensitivity =
                    features.SetupConflictRatio * (1f / Math.Max(1f, features.AvgEligibleMachines));

                features.MachinePreferenceSharpness =
                    features.MachineSpeedStd / Math.Max(1f, features.AvgProcessingTime);

                features.BottleneckLoadRatio =
                    features.SetupConflictRatio * features.AssignmentSensitivity;

                features.MachineLoadStd = features.MachineSpeedStd;
                features.BottleneckSensitivity =
                    features.BottleneckLoadRatio * features.MachinePreferenceSharpness;

                // Các thông số cứng (bạn có thể thay đổi cách lấy nếu cần)
                features.TightDueDateRatio = 0.4f;
                features.AvgSlackTime = 8f;
                features.CriticalJobRatio = 0.3f;

                // ====================================================================
                // 3. GỌI AI DỰ ĐOÁN (SAU KHI features ĐÃ ĐƯỢC TÍNH TOÁN FULL 100%)
                // ====================================================================
                var (aiTenure, aiNeighbors) = _tabuAIPredictor.PredictBestParameters(features);

                // Gắn "chốt an toàn" để đảm bảo hệ thống không bị crash ở biên
                aiTenure = Math.Clamp(aiTenure, 1, 100);
                aiNeighbors = Math.Max(aiNeighbors, (int)(currentJobCount * 2.5));

                // 4. Thiết lập Settings và cho Tabu Search chạy
                var aiSettings = new TabuSearchSettings
                {
                    UseAI = false,
                    RTS = true,
                    TabuTenure = aiTenure,
                    NumNeighborsPerMoAndStage = aiNeighbors,
                    MaxIterations = 2000,
                    MaxIdleIterations = 2000
                };


                Solution fixedResult = await tabuService.RunWithSettings(fixedInitial, fixedSettings, scenarioSeed);
                Solution aiResult = await tabuService.RunWithSettings(aiInitial, aiSettings, scenarioSeed);

                string logLine =
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
                    $"{currentJobCount}," +
                    $"{copperJobsCount}," +
                    $"{aluminumJobsCount}," +
                    $"{realMachineCount}," +
                    $"{FIXED_TENURE}," +
                    $"{FIXED_NEIGHBORS}," +
                    $"{fixedResult.Runtime.TotalMilliseconds:F2}," +
                    $"{fixedResult.BestObjective:F2}," +
                    $"{aiTenure}," +
                    $"{aiNeighbors}," +
                    $"{aiResult.Runtime.TotalMilliseconds:F2}," +
                    $"{aiResult.BestObjective:F2}\n";

                lock (fileLock)
                {
                    System.IO.File.AppendAllText(logFilePath, logLine, Encoding.UTF8);

                    Console.WriteLine(
                        $"[DONE] {currentJobCount} Jobs | " +
                        $"Fixed: {fixedResult.BestObjective:F2} | " +
                        $"AI: {aiResult.BestObjective:F2}");
                }

                summaryResults.Add(new
                {
                    TotalJobs = currentJobCount,
                    CopperJobs = copperJobsCount,
                    AluminumJobs = aluminumJobsCount,
                    ActiveMachines = realMachineCount,
                    FixedObjective = Math.Round(fixedResult.BestObjective, 2),
                    AIObjective = Math.Round(aiResult.BestObjective, 2),
                    FixedTime = $"{Math.Round(fixedResult.Runtime.TotalMilliseconds, 2)} ms",
                    AITime = $"{Math.Round(aiResult.Runtime.TotalMilliseconds, 2)} ms"
                });
            });

            return Ok(new
            {
                Message = "Hoàn tất Benchmark!",
                TotalScenarios = jobCountsToRun.Count,
                Details = summaryResults.OrderBy(x => (int)((dynamic)x).TotalJobs)
            });
        }
        [HttpPost("run-bruteforce-range")]
        public async Task<IActionResult> RunBruteForceRange([FromBody] BruteForceRangeRequest request)
        {
            if (request.StartLsx > request.EndLsx)
                return BadRequest("Số lệnh bắt đầu không được lớn hơn số lệnh kết thúc.");

            // 1. Tự động sinh danh sách mã lệnh (VD: từ 505 đến 508 -> LSX505, LSX506, LSX507, LSX508)
            int count = request.EndLsx - request.StartLsx + 1;
            var targetCodes = Enumerable.Range(request.StartLsx, count)
                                        .Select(i => $"LSX{i}")
                                        .ToList();

            // 2. Lấy dữ liệu từ Database
            var fetchedWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(targetCodes);
            var workOrders = fetchedWorkOrders?.ToList();

            if (workOrders == null || !workOrders.Any())
                return NotFound($"Không tìm thấy lệnh sản xuất nào trong dải LSX{request.StartLsx} - LSX{request.EndLsx}.");

            // 3. Lấy Máy móc khả dụng
            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments
                .Where(e => e.EquipmentStatus != EStatus.Breakdown
                         && e.EquipmentStatus != EStatus.Maintenance
                         && e.EquipmentStatus != EStatus.Pause)
                .ToList();

            List<Machine> machines = equipments.Select(eq => new Machine
            {
                Id = eq.Id,
                Code = eq.ResourceId ?? eq.Id.ToString()
            }).ToList();

            // 4. Chuyển đổi WorkOrder sang Job (Giữ nguyên logic của bạn)
            List<Job> jobs = new List<Job>();
            foreach (var wo in workOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;

                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0)
                            {
                                inputDiam = d;
                                break;
                            }
                        }
                    }
                }
                jobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            // 🚨 CHỐT CHẶN AN TOÀN CHO BRUTE FORCE
            int totalJobs = jobs.Count;
            if (totalJobs > 12)
            {
                return BadRequest($"CẢNH BÁO: Dải lệnh này có tổng cộng {totalJobs} Jobs. " +
                    $"Thuật toán Brute Force có độ phức tạp O(N!). Với {totalJobs} Jobs, máy chủ sẽ treo vì phải tính tới {Math.Round((double)Factorial(totalJobs), 0)} kịch bản. Vui lòng chọn dải lệnh nhỏ hơn (Tối đa ~10 Jobs).");
            }

            // 5. Khởi tạo Schedule và phân bổ máy
            var schedule = new MesMicroservice.Algorithm.Models.Schedule(
                machines,
                jobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList())
            );

            // 6. Chạy thuật toán vét cạn
            try
            {
                // Lấy service BruteForce từ DI Container
                var bruteForceAlg = _serviceProvider.GetRequiredService<MesMicroservice.Algorithm.BruteForces.BruteForce>();

                var result = await bruteForceAlg.Run(schedule);

                return Ok(new
                {
                    Message = $"Chạy thành công Brute Force cho {targetCodes.Count} lệnh ({totalJobs} Jobs).",
                    Objective = result.BestObjective,
                    Makespan = result.Makespan,
                    Iterations = result.Iterations,
                    RuntimeSeconds = result.Runtime.TotalSeconds,
                    Algorithm = result.Metaheuristic
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi chạy thuật toán: {ex.Message}");
            }
        }
        [HttpPost("automate-tabu-generation")]
        public async Task<IActionResult> AutomateTabu([FromBody] List<string> workOrderIds)
        {
            if (workOrderIds == null || workOrderIds.Count < 2)
                return BadRequest("Vui lòng truyền vào ít nhất 2 mã lệnh sản xuất.");

            var allWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(workOrderIds);
            if (allWorkOrders == null || !allWorkOrders.Any())
                return BadRequest("Không tìm thấy lệnh sản xuất nào trong Database.");

            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments.Where(e => e.EquipmentStatus != EStatus.Breakdown && e.EquipmentStatus != EStatus.Maintenance && e.EquipmentStatus != EStatus.Pause).ToList();

            string csvPath = Path.Combine(AppContext.BaseDirectory, "TabuSearch_Automated_Data.csv");
            if (!System.IO.File.Exists(csvPath))
            {
                string header = "TotalJobs,MachineCount,AvgEligibleMachines,MinEligibleMachines,MaxEligibleMachines,AvgProcessingTime,StdProcessingTime,AvgDiameter,DiameterVariance,AvgSetupTime,StdSetupTime,SetupConflictRatio,SameMaterialRatio,SameDiameterRatio,MachineSpeedStd,MachineEfficiencyStd,MachineCapabilityImbalance,AssignmentSensitivity,MachinePreferenceSharpness,BottleneckLoadRatio,MachineLoadStd,BottleneckSensitivity,TightDueDateRatio,AvgSlackTime,CriticalJobRatio,InitialMakespan,InitialObjective,BestObjective,RuntimeSeconds";
                await System.IO.File.WriteAllTextAsync(csvPath, header + Environment.NewLine);
            }

            List<Job> allJobs = new List<Job>();
            foreach (var wo in allWorkOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;
                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0) { inputDiam = d; break; }
                        }
                    }
                }
                allJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            if (allJobs.Count < 2) return BadRequest("Cần ít nhất 2 Jobs.");

            // =========================================================
            // CẤU HÌNH ĐA LUỒNG (PARALLEL)
            // =========================================================
            var jobCountsToRun = Enumerable.Range(2, Math.Max(0, allJobs.Count - 1)).ToList();
            var results = new System.Collections.Concurrent.ConcurrentBag<object>();
            object fileLock = new object();

            int maxThreads = Math.Max(1, Environment.ProcessorCount - 1);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            Console.WriteLine($"\n🚀 [Tabu-Parallel (AI Enabled)] Bắt đầu cày với {maxThreads} luồng...");

            await Parallel.ForEachAsync(jobCountsToRun, parallelOptions, async (currentCount, cancellationToken) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var scopedInsertion = scope.ServiceProvider.GetRequiredService<InsertionMethod>();
                var scopedTabu = scope.ServiceProvider.GetRequiredService<TabuSearchService>();
                var scopedEquipSelector = scope.ServiceProvider.GetRequiredService<IEquipmentSelectorService>();

                // 💡 BỔ SUNG: Gọi AI Predictor từ DI Container
                var scopedAIPredictor = scope.ServiceProvider.GetRequiredService<TabuAIPredictor>();

                try
                {
                    var subsetJobs = allJobs.Take(currentCount).Select(j => j.Clone()).ToList();
                    var mockMachines = equipments.Select(eq => new Machine { Id = eq.Id, Code = eq.ResourceId ?? eq.Id.ToString() }).ToList();
                    var jobDict = subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());

                    await scopedEquipSelector.AssignEligibleMachinesToJobsAsync(jobDict);

                    foreach (var job in subsetJobs)
                    {
                        if (job.EligibleMachines == null || !job.EligibleMachines.Any()) throw new Exception($"Job ID {job.Id} không có máy nào.");
                        foreach (var em in job.EligibleMachines)
                        {
                            var masterEq = equipments.FirstOrDefault(e => (e.ResourceId != null && e.ResourceId == em.Code) || (e.Id != null && e.Id.ToString() == em.Code));
                            if (masterEq != null) em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                        }
                    }

                    var sharedSchedule = new MesMicroservice.Algorithm.Models.Schedule(mockMachines, jobDict);
                    var baseSolution = await scopedInsertion.Run(sharedSchedule);

                    var features = AIFeatureExtractor.Extract(subsetJobs, mockMachines);
                    features.InitialMakespan = (float)baseSolution.Makespan;
                    features.InitialObjective = (float)baseSolution.BestObjective;

                    int setupConflict = 0; int sameMaterial = 0; int sameDiameter = 0;
                    for (int i = 0; i < subsetJobs.Count - 1; i++)
                    {
                        var a = subsetJobs[i]; var b = subsetJobs[i + 1];
                        if (a.MaterialDefinitionId != b.MaterialDefinitionId) setupConflict++; else sameMaterial++;
                        if (Math.Abs(a.Diameter - b.Diameter) < 0.01) sameDiameter++;
                    }
                    int transitions = Math.Max(1, subsetJobs.Count - 1);
                    features.SetupConflictRatio = (float)setupConflict / transitions;
                    features.SameMaterialRatio = (float)sameMaterial / transitions;
                    features.SameDiameterRatio = (float)sameDiameter / transitions;

                    // =======================================================
                    // 💡 1. YÊU CẦU AI DỰ ĐOÁN THÔNG SỐ TỐI ƯU
                    // =======================================================
                    var (aiTenure, aiNeighbors) = scopedAIPredictor.PredictBestParameters(features);
                    aiTenure = Math.Clamp(aiTenure, 1, 100);
                    aiNeighbors = Math.Max(aiNeighbors, (int)(currentCount * 2.5));

                    // =======================================================
                    // 💡 2. CẤU HÌNH BẬT AI VÀ RTS (REACTIVE TABU SEARCH)
                    // =======================================================
                    var tabuSettings = new TabuSearchSettings
                    {
                        UseAI = false,               // Bật AI
                        RTS = true,              // <-- Bỏ comment dòng này nếu biến bật Reactive Tabu Search của bạn tên là RTS
                        TabuTenure = aiTenure,
                        NumNeighborsPerMoAndStage = aiNeighbors,
                        MaxIterations = 2000,
                        MaxIdleIterations = 2000
                    };

                    var scheduleForTabu = new MesMicroservice.Algorithm.Models.Schedule(
                        mockMachines.Select(m => m.Clone()).ToList(),
                        subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList()));

                    // 💡 3. CHẠY VỚI CẤU HÌNH AI ĐÃ BẬT
                    var solution = await scopedTabu.RunWithSettings(scheduleForTabu, tabuSettings);

                    string rowData = string.Join(",",
                        features.TotalJobs, features.MachineCount, features.AvgEligibleMachines, features.MinEligibleMachines, features.MaxEligibleMachines, features.AvgProcessingTime, features.StdProcessingTime, features.AvgDiameter, features.DiameterVariance, features.AvgSetupTime, features.StdSetupTime, features.SetupConflictRatio, features.SameMaterialRatio, features.SameDiameterRatio, features.MachineSpeedStd, features.MachineEfficiencyStd, features.MachineCapabilityImbalance, features.AssignmentSensitivity, features.MachinePreferenceSharpness, features.BottleneckLoadRatio, features.MachineLoadStd, features.BottleneckSensitivity, features.TightDueDateRatio, features.AvgSlackTime, features.CriticalJobRatio, features.InitialMakespan, features.InitialObjective,
                        solution.BestObjective, solution.Runtime.TotalSeconds);

                    lock (fileLock)
                    {
                        System.IO.File.AppendAllText(csvPath, rowData + Environment.NewLine);
                    }

                    results.Add(new { Jobs = currentCount, InitObj = Math.Round(baseSolution.BestObjective, 0), BestObj = Math.Round(solution.BestObjective, 0), TimeSeconds = Math.Round(solution.Runtime.TotalSeconds, 3) });
                    Console.WriteLine($"[Tabu + AI] Xong {currentCount} Jobs trong {solution.Runtime.TotalSeconds:F3}s - AI chọn (T={aiTenure}, N={aiNeighbors})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Tabu Error] {currentCount} Jobs: {ex.Message}");
                    results.Add(new { Jobs = currentCount, Error = ex.Message });
                }
            });

            return Ok(new { Message = $"Đã chạy xong Tabu Đa luồng (Có AI)!", SavedAt = csvPath, Summary = results.OrderBy(x => ((dynamic)x).Jobs) });
        }
        [HttpPost("compare-tabu-ml-rts-vs-ga")]
        public async Task<IActionResult> CompareTabuMlRtsVsGa([FromBody] List<string> workOrderIds)
        {
            if (workOrderIds == null || workOrderIds.Count < 2)
                return BadRequest("Vui lòng truyền vào ít nhất 2 mã lệnh sản xuất.");

            var allWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(workOrderIds);
            if (allWorkOrders == null || !allWorkOrders.Any())
                return BadRequest("Không tìm thấy dữ liệu WorkOrder.");

            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments
                .Where(e => e.EquipmentStatus != EStatus.Breakdown
                         && e.EquipmentStatus != EStatus.Maintenance
                         && e.EquipmentStatus != EStatus.Pause)
                .ToList();

            if (!equipments.Any())
                return BadRequest("Không có máy khả dụng.");

            var allJobs = new List<Job>();
            foreach (var wo in allWorkOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;

                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef == null) continue;

                        var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                        if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0)
                        {
                            inputDiam = d;
                            break;
                        }
                    }
                }

                allJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            if (allJobs.Count < 2)
                return BadRequest("Cần ít nhất 2 Jobs để so sánh.");

            var machines = equipments
                .Select(eq => MachineMapper.FromEquipment(eq, availableTime: 0))
                .ToList();

            string logFilePath = Path.Combine(AppContext.BaseDirectory, "Compare_TabuMLRTS_vs_GA_Reevaluated.csv");
            if (!System.IO.File.Exists(logFilePath))
            {
                string header =
                    "Timestamp,TotalJobs,MachineCount,InitialObjective,InitialMakespan," +
                    "AI_Tenure,AI_Neighbors," +
                    "Tabu_Runtime_ms,Tabu_Objective,Tabu_Makespan,Tabu_Setup,Tabu_Lateness," +
                    "GA_Runtime_ms,GA_Objective,GA_Makespan,GA_Setup,GA_Lateness," +
                    "Winner,ObjectiveGap,TabuBetterPercent";

                await System.IO.File.WriteAllTextAsync(logFilePath, header + Environment.NewLine, Encoding.UTF8);
            }

            var summaryResults = new System.Collections.Concurrent.ConcurrentBag<object>();
            object fileLock = new object();

            // GA bên trong đã dùng ParallelTaskExecutor, nên để mức này tránh nghẽn CPU.
            int maxThreads = Math.Max(1, Environment.ProcessorCount - 1);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            var jobCountsToRun = Enumerable.Range(2, Math.Max(0, allJobs.Count - 1)).ToList();

            await Parallel.ForEachAsync(jobCountsToRun, parallelOptions, async (currentJobCount, cancellationToken) =>
            {
                using var scope = _serviceProvider.CreateScope();

                var insertionMethod = scope.ServiceProvider.GetRequiredService<InsertionMethod>();
                var tabuService = scope.ServiceProvider.GetRequiredService<TabuSearchService>();
                var gaService = scope.ServiceProvider.GetRequiredService<MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm.GAService>();
                var equipmentSelector = scope.ServiceProvider.GetRequiredService<IEquipmentSelectorService>();
                var aiPredictor = scope.ServiceProvider.GetRequiredService<TabuAIPredictor>();

                try
                {
                    var scenarioJobs = allJobs.Take(currentJobCount).Select(j => j.Clone()).ToList();
                    var scenarioMachines = machines.Select(m => m.Clone()).ToList();

                    var scenarioJobDict = scenarioJobs
                        .GroupBy(j => j.ManufacturingOrderId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    await equipmentSelector.AssignEligibleMachinesToJobsAsync(scenarioJobDict);

                    foreach (var job in scenarioJobs)
                    {
                        if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                            throw new Exception($"Job ID {job.Id} không có máy khả dụng.");

                        foreach (var em in job.EligibleMachines)
                        {
                            var masterEq = equipments.FirstOrDefault(e =>
                                (e.ResourceId != null && e.ResourceId == em.Code) ||
                                (e.Id != null && e.Id.ToString() == em.Code));

                            if (masterEq != null)
                                em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                        }
                    }

                    var baseSchedule = new MesMicroservice.Algorithm.Models.Schedule(
                        scenarioMachines.Select(m => m.Clone()).ToList(),
                        scenarioJobs.GroupBy(j => j.ManufacturingOrderId)
                            .ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList()));

                    
                    var assignmentMethod = scope.ServiceProvider.GetRequiredService<IAssignmentMethod>();

                    var baseInsertionSolution = await insertionMethod.Run(baseSchedule);
                    var baseSolution = await assignmentMethod.Run(baseInsertionSolution.BestSchedule);
                    var evaluator = new ObjectiveEvaluator();
                    var baseScheduleForMetrics = baseSolution.BestSchedule.Clone();

                    double initialObjective = evaluator.Evaluate(baseScheduleForMetrics);
                    double initialMakespan = evaluator.CalculateMakespan(baseScheduleForMetrics);

                    var features = AIFeatureExtractor.Extract(scenarioJobs, scenarioMachines);
                    features.InitialMakespan = (float)initialMakespan;
                    features.InitialObjective = (float)initialObjective;

                    var (aiTenure, aiNeighbors) = aiPredictor.PredictBestParameters(features);
                    aiTenure = Math.Clamp(aiTenure, 3, 80);
                    aiNeighbors = Math.Clamp(Math.Max(aiNeighbors, (int)Math.Ceiling(currentJobCount * 2.5)), 20, 500);

                    var tabuSettings = new TabuSearchSettings
                    {
                        UseAI = false,
                        RTS = true,
                        TabuTenure = aiTenure,
                        NumNeighborsPerMoAndStage = aiNeighbors,
                        MaxIterations = 2000,
                        MaxIdleIterations = 2000
                    };

                    int scenarioSeed = CreateScenarioSeed(currentJobCount, workOrderIds);

                    var tabuSchedule = new MesMicroservice.Algorithm.Models.Schedule(
                        scenarioMachines.Select(m => m.Clone()).ToList(),
                        scenarioJobs.GroupBy(j => j.ManufacturingOrderId)
                            .ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList()));

                    var gaSchedule = new MesMicroservice.Algorithm.Models.Schedule(
                        scenarioMachines.Select(m => m.Clone()).ToList(),
                        scenarioJobs.GroupBy(j => j.ManufacturingOrderId)
                            .ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList()));

                    var tabuResult = await tabuService.RunWithSettings(tabuSchedule, tabuSettings, scenarioSeed);
                    var gaResult = await gaService.Run(gaSchedule);

                    var tabuEvaluatedSchedule = tabuResult.BestSchedule.Clone();
                    var gaEvaluatedSchedule = gaResult.BestSchedule.Clone();

                    double tabuObjective = evaluator.Evaluate(tabuEvaluatedSchedule);
                    double gaObjective = evaluator.Evaluate(gaEvaluatedSchedule);

                    double tabuMakespan = evaluator.CalculateMakespan(tabuEvaluatedSchedule);
                    double gaMakespan = evaluator.CalculateMakespan(gaEvaluatedSchedule);
                    double tabuSetup = evaluator.CalculateTotalSetupTime(tabuEvaluatedSchedule);
                    double gaSetup = evaluator.CalculateTotalSetupTime(gaEvaluatedSchedule);
                    double tabuLateness = evaluator.CalculateTotalLateness(tabuEvaluatedSchedule);
                    double gaLateness = evaluator.CalculateTotalLateness(gaEvaluatedSchedule);

                    string winner = tabuObjective < gaObjective
                        ? "TabuML_RTS"
                        : gaObjective < tabuObjective ? "GA" : "Tie";

                    double objectiveGap = tabuObjective - gaObjective;
                    double tabuBetterPercent = gaObjective > 0
                        ? ((gaObjective - tabuObjective) / gaObjective) * 100.0
                        : 0;

                    string logLine =
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{currentJobCount},{scenarioMachines.Count}," +
                        $"{initialObjective:F2},{initialMakespan:F2},{aiTenure},{aiNeighbors}," +
                        $"{tabuResult.Runtime.TotalMilliseconds:F2},{tabuObjective:F2},{tabuMakespan:F2},{tabuSetup:F2},{tabuLateness:F2}," +
                        $"{gaResult.Runtime.TotalMilliseconds:F2},{gaObjective:F2},{gaMakespan:F2},{gaSetup:F2},{gaLateness:F2}," +
                        $"{winner},{objectiveGap:F2},{tabuBetterPercent:F2}";

                    lock (fileLock)
                    {
                        System.IO.File.AppendAllText(logFilePath, logLine + Environment.NewLine, Encoding.UTF8);
                    }

                    summaryResults.Add(new
                    {
                        TotalJobs = currentJobCount,
                        MachineCount = scenarioMachines.Count,
                        InitialObjective = Math.Round(initialObjective, 2),
                        AiTenure = aiTenure,
                        AiNeighbors = aiNeighbors,
                        InitialMakespan = Math.Round(initialMakespan, 2),
                        TabuMlRtsObjective = Math.Round(tabuObjective, 2),
                        GaObjective = Math.Round(gaObjective, 2),
                        TabuMakespan = Math.Round(tabuMakespan, 2),
                        GaMakespan = Math.Round(gaMakespan, 2),
                        TabuSetup = Math.Round(tabuSetup, 2),
                        GaSetup = Math.Round(gaSetup, 2),
                        TabuLateness = Math.Round(tabuLateness, 2),
                        GaLateness = Math.Round(gaLateness, 2),
                        Winner = winner,
                        ObjectiveGap = Math.Round(objectiveGap, 2),
                        TabuBetterPercent = Math.Round(tabuBetterPercent, 2),
                        TabuRuntimeMs = Math.Round(tabuResult.Runtime.TotalMilliseconds, 2),
                        GaRuntimeMs = Math.Round(gaResult.Runtime.TotalMilliseconds, 2)
                    });
                }
                catch (Exception ex)
                {
                    summaryResults.Add(new { TotalJobs = currentJobCount, Error = ex.Message });
                }
            });

            return Ok(new
            {
                Message = "Hoàn tất so sánh Tabu Search ML.NET + RTS với GA.",
                SavedAt = logFilePath,
                TotalScenarios = jobCountsToRun.Count,
                MaxDegreeOfParallelism = maxThreads,
                Details = summaryResults.OrderBy(x => (int)((dynamic)x).TotalJobs)
            });
        }

        [HttpPost("automate-ga-generation")]
        public async Task<IActionResult> AutomateGA([FromBody] List<string> workOrderIds)
        {
            if (workOrderIds == null || workOrderIds.Count < 2)
                return BadRequest("Vui lòng truyền vào ít nhất 2 mã lệnh sản xuất.");

            var allWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(workOrderIds);
            if (allWorkOrders == null || !allWorkOrders.Any())
                return BadRequest("Không tìm thấy lệnh sản xuất nào trong Database.");

            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments.Where(e => e.EquipmentStatus != EStatus.Breakdown && e.EquipmentStatus != EStatus.Maintenance && e.EquipmentStatus != EStatus.Pause).ToList();

            string csvPath = Path.Combine(AppContext.BaseDirectory, "GA_Automated_Data.csv");
            if (!System.IO.File.Exists(csvPath))
            {
                string header = "TotalJobs,MachineCount,AvgEligibleMachines,MinEligibleMachines,MaxEligibleMachines,AvgProcessingTime,StdProcessingTime,AvgDiameter,DiameterVariance,AvgSetupTime,StdSetupTime,SetupConflictRatio,SameMaterialRatio,SameDiameterRatio,MachineSpeedStd,MachineEfficiencyStd,MachineCapabilityImbalance,AssignmentSensitivity,MachinePreferenceSharpness,BottleneckLoadRatio,MachineLoadStd,BottleneckSensitivity,TightDueDateRatio,AvgSlackTime,CriticalJobRatio,InitialMakespan,InitialObjective,Neighbors,Tenure,BestObjective,RuntimeSeconds";
                await System.IO.File.WriteAllTextAsync(csvPath, header + Environment.NewLine);
            }

            List<Job> allJobs = new List<Job>();
            foreach (var wo in allWorkOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;
                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0) { inputDiam = d; break; }
                        }
                    }
                }
                allJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            if (allJobs.Count < 2) return BadRequest("Cần ít nhất 2 Jobs.");

            // =========================================================
            // CẤU HÌNH ĐA LUỒNG (PARALLEL)
            // =========================================================
            var jobCountsToRun = Enumerable.Range(2, Math.Max(0, allJobs.Count - 1)).ToList();
            var results = new System.Collections.Concurrent.ConcurrentBag<object>();
            object fileLock = new object();

            int maxThreads = Math.Max(1, Environment.ProcessorCount - 1);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

            Console.WriteLine($"\n🚀 [GA-Parallel] Bắt đầu cày với {maxThreads} luồng đồng thời...");

            await Parallel.ForEachAsync(jobCountsToRun, parallelOptions, async (currentCount, cancellationToken) =>
            {
                // QUAN TRỌNG: Mở Scope mới cho mỗi luồng
                using var scope = _serviceProvider.CreateScope();
                var scopedInsertion = scope.ServiceProvider.GetRequiredService<InsertionMethod>();
                // Lấy Service GA
                var scopedGA = scope.ServiceProvider.GetRequiredService<MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm.GAService>();
                var scopedEquipSelector = scope.ServiceProvider.GetRequiredService<IEquipmentSelectorService>();

                try
                {
                    var subsetJobs = allJobs.Take(currentCount).Select(j => j.Clone()).ToList();
                    var mockMachines = equipments.Select(eq => new Machine { Id = eq.Id, Code = eq.ResourceId ?? eq.Id.ToString() }).ToList();
                    var jobDict = subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());

                    await scopedEquipSelector.AssignEligibleMachinesToJobsAsync(jobDict);

                    foreach (var job in subsetJobs)
                    {
                        if (job.EligibleMachines == null || !job.EligibleMachines.Any()) throw new Exception($"Job ID {job.Id} không có máy nào.");
                        foreach (var em in job.EligibleMachines)
                        {
                            var masterEq = equipments.FirstOrDefault(e => (e.ResourceId != null && e.ResourceId == em.Code) || (e.Id != null && e.Id.ToString() == em.Code));
                            if (masterEq != null) em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                        }
                    }

                    var sharedSchedule = new MesMicroservice.Algorithm.Models.Schedule(mockMachines, jobDict);
                    var baseSolution = await scopedInsertion.Run(sharedSchedule);

                    var features = AIFeatureExtractor.Extract(subsetJobs, mockMachines);
                    features.InitialMakespan = (float)baseSolution.Makespan;
                    features.InitialObjective = (float)baseSolution.BestObjective;

                    int setupConflict = 0; int sameMaterial = 0; int sameDiameter = 0;
                    for (int i = 0; i < subsetJobs.Count - 1; i++)
                    {
                        var a = subsetJobs[i]; var b = subsetJobs[i + 1];
                        if (a.MaterialDefinitionId != b.MaterialDefinitionId) setupConflict++; else sameMaterial++;
                        if (Math.Abs(a.Diameter - b.Diameter) < 0.01) sameDiameter++;
                    }
                    int transitions = Math.Max(1, subsetJobs.Count - 1);
                    features.SetupConflictRatio = (float)setupConflict / transitions;
                    features.SameMaterialRatio = (float)sameMaterial / transitions;
                    features.SameDiameterRatio = (float)sameDiameter / transitions;

                    // ===== CHẠY GENETIC ALGORITHM (GA) =====
                    var scheduleForGA = new MesMicroservice.Algorithm.Models.Schedule(
                        mockMachines.Select(m => m.Clone()).ToList(),
                        subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList()));

                    var solution = await scopedGA.Run(scheduleForGA);

                    string rowData = string.Join(",",
                        features.TotalJobs, features.MachineCount, features.AvgEligibleMachines, features.MinEligibleMachines, features.MaxEligibleMachines, features.AvgProcessingTime, features.StdProcessingTime, features.AvgDiameter, features.DiameterVariance, features.AvgSetupTime, features.StdSetupTime, features.SetupConflictRatio, features.SameMaterialRatio, features.SameDiameterRatio, features.MachineSpeedStd, features.MachineEfficiencyStd, features.MachineCapabilityImbalance, features.AssignmentSensitivity, features.MachinePreferenceSharpness, features.BottleneckLoadRatio, features.MachineLoadStd, features.BottleneckSensitivity, features.TightDueDateRatio, features.AvgSlackTime, features.CriticalJobRatio, features.InitialMakespan, features.InitialObjective,
                        solution.BestObjective, solution.Runtime.TotalSeconds);

                    // Khóa (lock) để tránh lỗi đụng độ file
                    lock (fileLock)
                    {
                        System.IO.File.AppendAllText(csvPath, rowData + Environment.NewLine);
                    }

                    results.Add(new { Jobs = currentCount, InitObj = Math.Round(baseSolution.BestObjective, 0), BestObj = Math.Round(solution.BestObjective, 0), TimeSeconds = Math.Round(solution.Runtime.TotalSeconds, 3) });
                    Console.WriteLine($"[GA] Xong {currentCount} Jobs trong {solution.Runtime.TotalSeconds:F3}s");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GA Error] {currentCount} Jobs: {ex.Message}");
                    results.Add(new { Jobs = currentCount, Error = ex.Message });
                }
            });

            return Ok(new { Message = $"Đã chạy xong GA Đa luồng!", SavedAt = csvPath, Summary = results.OrderBy(x => ((dynamic)x).Jobs) });
        }

        
        [HttpPost("automate-bruteforce-generation")]
        public async Task<IActionResult> AutomateBruteForce([FromBody] List<string> workOrderIds)
        {
            if (workOrderIds == null || workOrderIds.Count < 2)
                return BadRequest("Vui lòng truyền vào ít nhất 2 mã lệnh sản xuất.");

            // =========================================================
            // 1. LẤY DỮ LIỆU TỪ DATABASE
            // =========================================================
            var allWorkOrders = await _workOrderRepository.GetByWorkOrderCodesAsync(workOrderIds);
            if (allWorkOrders == null || !allWorkOrders.Any())
                return BadRequest("Không tìm thấy lệnh sản xuất nào trong Database.");

            var allEquipments = await _equipmentRepository.GetAllAsync();
            var equipments = allEquipments
                .Where(e => e.EquipmentStatus != EStatus.Breakdown
                         && e.EquipmentStatus != EStatus.Maintenance
                         && e.EquipmentStatus != EStatus.Pause)
                .ToList();

            // =========================================================
            // 2. KHỞI TẠO FILE CSV (HEADER MỚI CHUẨN)
            // =========================================================
            string csvPath = Path.Combine(AppContext.BaseDirectory, "BruteForce_Data_Ultimate.csv");
            if (!System.IO.File.Exists(csvPath))
            {
                string header = "TotalJobs,MachineCount,AvgEligibleMachines,MinEligibleMachines,MaxEligibleMachines,AvgProcessingTime,StdProcessingTime,AvgDiameter,DiameterVariance,AvgSetupTime,StdSetupTime,SetupConflictRatio,SameMaterialRatio,SameDiameterRatio,MachineSpeedStd,MachineEfficiencyStd,MachineCapabilityImbalance,AssignmentSensitivity,MachinePreferenceSharpness,BottleneckLoadRatio,MachineLoadStd,BottleneckSensitivity,TightDueDateRatio,AvgSlackTime,CriticalJobRatio,InitialMakespan,InitialObjective,BestObjective,RuntimeSeconds";
                await System.IO.File.WriteAllTextAsync(csvPath, header + Environment.NewLine);
            }

            // =========================================================
            // 3. CHUYỂN ĐỔI DỮ LIỆU SANG JOB
            // =========================================================
            List<Job> allJobs = new List<Job>();
            foreach (var wo in allWorkOrders)
            {
                var prodParam = await _productionParameterRepository.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;

                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepository.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0)
                            {
                                inputDiam = d;
                                break;
                            }
                        }
                    }
                }
                allJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            if (allJobs.Count < 2)
                return BadRequest("Không đủ số lượng Jobs hợp lệ để chạy thuật toán (Cần ít nhất 2 Jobs).");

            // =========================================================
            // 4. CHẠY VÒNG LẶP (TỪ 2 ĐẾN MAX 10 JOBS)
            // =========================================================
            int maxSafetyLimit = Math.Min(allJobs.Count, 15);
            var results = new List<object>();

            for (int currentCount = 2; currentCount <= maxSafetyLimit; currentCount++)
            {
                try
                {
                    var subsetJobs = allJobs.Take(currentCount).Select(j => j.Clone()).ToList();

                    var mockMachines = equipments.Select(eq => new Machine
                    {
                        Id = eq.Id,
                        Code = eq.ResourceId ?? eq.Id.ToString()
                    }).ToList();

                    var jobDict = subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());
                    await _equipmentSelectorService.AssignEligibleMachinesToJobsAsync(jobDict);

                    // ĐỒNG BỘ MÃ MÁY VÀ BẮT LỖI
                    foreach (var job in subsetJobs)
                    {
                        if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                            throw new Exception($"Job ID {job.Id} không có máy nào khả dụng để chạy!");

                        foreach (var em in job.EligibleMachines)
                        {
                            var masterEq = equipments.FirstOrDefault(e => (e.ResourceId != null && e.ResourceId == em.Code) || (e.Id != null && e.Id.ToString() == em.Code));
                            if (masterEq != null) em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                        }
                    }

                    // A. TÍNH TOÁN BASE OBJECTIVE BẰNG INSERTION METHOD
                    var sharedSchedule = new MesMicroservice.Algorithm.Models.Schedule(mockMachines, jobDict);
                    var insertionMethod = _serviceProvider.GetRequiredService<InsertionMethod>();
                    var baseSolution = await insertionMethod.Run(sharedSchedule);

                    // B. TRÍCH XUẤT 27 CỘT FEATURES
                    var features = AIFeatureExtractor.Extract(subsetJobs, mockMachines);
                    features.InitialMakespan = (float)baseSolution.Makespan;
                    features.InitialObjective = (float)baseSolution.BestObjective;

                    // Bổ sung các hệ số tỷ lệ phức tạp
                    int setupConflict = 0; int sameMaterial = 0; int sameDiameter = 0;
                    for (int i = 0; i < subsetJobs.Count - 1; i++)
                    {
                        var a = subsetJobs[i]; var b = subsetJobs[i + 1];
                        if (a.MaterialDefinitionId != b.MaterialDefinitionId) setupConflict++;
                        else sameMaterial++;
                        if (Math.Abs(a.Diameter - b.Diameter) < 0.01) sameDiameter++;
                    }
                    int transitions = Math.Max(1, subsetJobs.Count - 1);
                    features.SetupConflictRatio = (float)setupConflict / transitions;
                    features.SameMaterialRatio = (float)sameMaterial / transitions;
                    features.SameDiameterRatio = (float)sameDiameter / transitions;

                    // C. GỌI BRUTE FORCE CHẠY VÀ TÌM NGHIỆM
                    var bruteForceAlg = _serviceProvider.GetRequiredService<MesMicroservice.Algorithm.BruteForces.BruteForce>();

                    var scheduleForBF = new MesMicroservice.Algorithm.Models.Schedule(
                        mockMachines.Select(m => m.Clone()).ToList(),
                        subsetJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList())
                    );

                    var solution = await bruteForceAlg.Run(scheduleForBF);

                    // D. GHI VÀO CSV (CHẮC CHẮN GHI BEST OBJECTIVE VÀ RUNTIME)
                    string rowData = string.Join(",",
                        features.TotalJobs, features.MachineCount, features.AvgEligibleMachines, features.MinEligibleMachines,
                        features.MaxEligibleMachines, features.AvgProcessingTime, features.StdProcessingTime,
                        features.AvgDiameter, features.DiameterVariance, features.AvgSetupTime, features.StdSetupTime,
                        features.SetupConflictRatio, features.SameMaterialRatio, features.SameDiameterRatio,
                        features.MachineSpeedStd, features.MachineEfficiencyStd, features.MachineCapabilityImbalance,
                        features.AssignmentSensitivity, features.MachinePreferenceSharpness, features.BottleneckLoadRatio,
                        features.MachineLoadStd, features.BottleneckSensitivity, features.TightDueDateRatio,
                        features.AvgSlackTime, features.CriticalJobRatio,
                        features.InitialMakespan,
                        features.InitialObjective,
                        solution.BestObjective,
                        solution.Runtime.TotalSeconds
                    );

                    await System.IO.File.AppendAllTextAsync(csvPath, rowData + Environment.NewLine);

                    results.Add(new
                    {
                        Jobs = currentCount,
                        InitObj = Math.Round(baseSolution.BestObjective, 0),
                        BestObj = Math.Round(solution.BestObjective, 0),
                        TimeSeconds = Math.Round(solution.Runtime.TotalSeconds, 3)
                    });

                    Console.WriteLine($"[BruteForce] Xong {currentCount} Jobs trong {solution.Runtime.TotalSeconds:F3}s - Init: {baseSolution.BestObjective:F0} -> Best: {solution.BestObjective:F0}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BruteForce Error] Lỗi ở kịch bản {currentCount} Jobs: {ex.Message}");
                    results.Add(new { Jobs = currentCount, Error = ex.Message });
                }
            }

            // ĐÂY CHÍNH LÀ ĐOẠN BẠN BỊ THIẾU KHIẾN NÓ BÁO LỖI "NOT ALL CODE PATHS RETURN A VALUE"
            return Ok(new
            {
                Message = $"Đã cày xong dữ liệu Brute Force (từ 2 đến {maxSafetyLimit} Jobs)!",
                SavedAt = csvPath,
                Summary = results
            });
        }

        // Hàm tính giai thừa (Dùng để hiển thị cảnh báo)
        private static double Factorial(int n)
        {
            if (n <= 1) return 1;
            double result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            return result;
        }

        // Request Model (Khai báo ở cuối file Controller)
        public class BruteForceRangeRequest
        {
            public int StartLsx { get; set; } // Ví dụ: 505
            public int EndLsx { get; set; }   // Ví dụ: 507
        }


        public class ScheduleRequest
        {
            public List<JobDto> Jobs { get; set; } = new List<JobDto>();
        }
        public class CustomBenchmarkRequest
        {
            public List<string> WorkOrderIds { get; set; } = new List<string>();

            /// <summary>
            /// Số lượng lệnh sản xuất muốn lấy ra chạy. Ví dụ: 20
            /// </summary>
            public int TargetJobCount { get; set; }

            /// <summary>
            /// Số lần lặp lại kịch bản này. Ví dụ: 50 (sẽ sinh ra 50 dòng trong CSV)
            /// </summary>
            public int NumberOfRuns { get; set; }
        }

        public class JobDto
        {
            public string? Name { get; set; }
            public double ProcessingTime { get; set; }
        }
        private static int CreateScenarioSeed(int currentJobCount, IReadOnlyList<string> workOrderIds)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + currentJobCount;
                hash = hash * 31 + workOrderIds.Count;

                foreach (var id in workOrderIds)
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    foreach (char c in id)
                    {
                        hash = hash * 31 + c;
                    }
                }

                return hash & int.MaxValue;
            }
        }
    }
}