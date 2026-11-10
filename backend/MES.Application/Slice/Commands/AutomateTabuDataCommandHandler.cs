using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MesMicroservice.Algorithm.Models;
using MesMicroservice.Algorithm.Models.Mapping;
using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
using MesMicroservice.Algorithm.Services;
using MesMicroservice.Domain.Enums;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MesMicroservice.Application.Interfaces;

namespace MesMicroservice.Application.Commands.TabuData
{
    public class AutomateTabuDataCommandHandler : IRequestHandler<AutomateTabuDataCommand, string>
    {
        private readonly IWorkOrderRepository _workOrderRepo;
        private readonly IEquipmentRepository _equipmentRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEquipmentSelectorService _equipmentSelectorService;
        private readonly IProductionParameterRepository _productionParameterRepo;
        private readonly IMaterialDefinitionRepository _materialDefinitionRepo;
        private static readonly object _fileLock = new object();

        public AutomateTabuDataCommandHandler(
            IWorkOrderRepository workOrderRepo,
            IEquipmentRepository equipmentRepo,
            IServiceProvider serviceProvider,
            IEquipmentSelectorService equipmentSelectorService,
            IProductionParameterRepository productionParameterRepo,
            IMaterialDefinitionRepository materialDefinitionRepo)
        {
            _workOrderRepo = workOrderRepo;
            _equipmentRepo = equipmentRepo;
            _serviceProvider = serviceProvider;
            _equipmentSelectorService = equipmentSelectorService;
            _productionParameterRepo = productionParameterRepo;
            _materialDefinitionRepo = materialDefinitionRepo;
        }

        public async Task<string> Handle(AutomateTabuDataCommand request, CancellationToken cancellationToken)
        {
            // 1. CHUẨN BỊ DỮ LIỆU GỐC
            var workOrders = await _workOrderRepo.GetByWorkOrderCodesAsync(request.WorkOrderIds);
            var allEquipments = await _equipmentRepo.GetAllAsync();

            if (workOrders == null || !workOrders.Any())
                throw new Exception("Không tìm thấy dữ liệu WorkOrder.");

            // Lọc máy chuẩn Domain: Chỉ lấy Available (8) và Running (9)
            var activeEquipments = allEquipments.Where(eq =>
                eq.EquipmentStatus == EStatus.Available ||
                eq.EquipmentStatus == EStatus.Running
               ).ToList();

            List<Machine> baseMachines = activeEquipments.Select(eq => new Machine
            {
                Id = eq.Id,
                Code = eq.ResourceId ?? eq.Id.ToString()
            }).ToList();

            Console.WriteLine($"[Trạng thái DB] Tổng: {allEquipments.Count()} máy | Sẵn sàng chạy: {activeEquipments.Count} máy (Available/Running)");

            // Mapping sơ bộ Job
            List<Job> baseJobs = new List<Job>();
            foreach (var wo in workOrders)
            {
                var prodParam = await _productionParameterRepo.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                var prodProps = prodParam?.Properties;
                string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                double inputDiam = 0;
                if (wo.OperationsSegment?.MaterialSpecifications != null)
                {
                    foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                    {
                        var matDef = await _materialDefinitionRepo.GetAsync(maSpec.MaterialDefinitionId ?? "");
                        if (matDef != null)
                        {
                            var kcProp = matDef.Properties.FirstOrDefault(p => p.Name.Equals("K/C", StringComparison.OrdinalIgnoreCase));
                            if (double.TryParse(kcProp?.Value?.ValueString, out var d) && d > 0) { inputDiam = d; break; }
                        }
                    }
                }
                baseJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
            }

            // 2. TẠO DANH SÁCH CÁC TỔ HỢP THAM SỐ
            var combinations = new List<(int JobCount, int Tenure, int Neighbors)>();

            // Chạy số lượng lệnh từ 2 đến số lệnh tối đa
            for (int jobCount = 2; jobCount <= baseJobs.Count; jobCount++)
            {
                for (int t = request.MinTenure; t <= request.MaxTenure; t += request.TenureStep)
                {
                    for (int n = request.MinNeighbors; n <= request.MaxNeighbors; n += request.NeighborsStep)
                    {
                        for (int r = 0; r < request.RunsPerCombination; r++)
                        {
                            combinations.Add((jobCount, t, n));
                        }
                    }
                }
            }

            // ĐÓNG GÓI JSON ĐỂ DEEP CLONE (Chống lỗi đa luồng)
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            string jsonBaseMachines = System.Text.Json.JsonSerializer.Serialize(baseMachines, jsonOptions);
            string jsonBaseJobs = System.Text.Json.JsonSerializer.Serialize(baseJobs, jsonOptions);

            // ========================================================
            // 3. CHẠY SONG SONG
            // ========================================================
            int totalProcessed = 0;
            await Parallel.ForEachAsync(combinations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (combo, ct) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // 3.1. Giải mã JSON để có đối tượng độc lập
                    var machines = System.Text.Json.JsonSerializer.Deserialize<List<Machine>>(jsonBaseMachines, jsonOptions);
                    var allDeserializedJobs = System.Text.Json.JsonSerializer.Deserialize<List<Job>>(jsonBaseJobs, jsonOptions);

                    // 3.2. CẮT ĐÚNG SỐ LƯỢNG LỆNH CỦA VÒNG LẶP NÀY
                    var jobs = allDeserializedJobs.Take(combo.JobCount).ToList();

                    // 3.3. Đếm số lượng Đồng/Nhôm (KHAI BÁO BIẾN Ở ĐÂY)
                    int copperJobsCount = jobs.Count(j =>
                        !string.IsNullOrEmpty(j.Name) &&
                        (
                            j.Name.Contains("Cu", StringComparison.Ordinal) ||
                            j.Name.Contains("CU", StringComparison.Ordinal) ||
                            j.Name.Contains("Đồng", StringComparison.OrdinalIgnoreCase) ||
                            j.Name.Contains("Dong", StringComparison.OrdinalIgnoreCase)
                        )
                    );

                    int aluminumJobsCount = jobs.Count(j =>
                        !string.IsNullOrEmpty(j.Name) &&
                        (
                            j.Name.Contains("Al", StringComparison.Ordinal) ||
                            j.Name.Contains("AL", StringComparison.Ordinal) ||
                            j.Name.Contains("Nhôm", StringComparison.OrdinalIgnoreCase) ||
                            j.Name.Contains("Nhom", StringComparison.OrdinalIgnoreCase)
                        )
                    );

                    // 3.4. Gán máy và chạy logic
                    var jobDict = jobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());

                    var equipmentSelector = scope.ServiceProvider.GetRequiredService<IEquipmentSelectorService>();
                    await equipmentSelector.AssignEligibleMachinesToJobsAsync(jobDict);

                    var activeMachineCodes = machines.Select(m => m.Code?.Trim().ToUpper()).ToHashSet();

                    foreach (var job in jobs)
                    {
                        if (job.EligibleMachines == null || !job.EligibleMachines.Any())
                        {
                            throw new Exception($"[LỖI] Lệnh '{job.ManufacturingOrderId}' không tìm thấy máy hỗ trợ.");
                        }

                        foreach (var em in job.EligibleMachines)
                        {
                            var masterEq = activeEquipments.FirstOrDefault(e =>
                                e.ResourceId?.Trim().ToUpper() == em.Code?.Trim().ToUpper() ||
                                e.Id.ToString() == em.Code?.Trim().ToUpper());

                            if (masterEq != null) em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                        }

                        job.EligibleMachines = job.EligibleMachines
                            .Where(em => activeMachineCodes.Contains(em.Code?.Trim().ToUpper()))
                            .ToList();

                        if (!job.EligibleMachines.Any())
                        {
                            throw new Exception($"[LỖI] Lệnh '{job.ManufacturingOrderId}' máy hỗ trợ đều đang hỏng/bảo trì.");
                        }
                    }

                    // 3.5. Chạy thuật toán Tabu Search
                    var initialSchedule = new Schedule(machines, jobDict);
                    var insertionMethod = scope.ServiceProvider.GetRequiredService<InsertionMethod>();
                    var baseSol = await insertionMethod.Run(initialSchedule);

                    var tabuService = scope.ServiceProvider.GetRequiredService<TabuSearchService>();
                    var settings = new TabuSearchSettings { TabuTenure = combo.Tenure, NumNeighborsPerMoAndStage = combo.Neighbors, MaxIterations = 1000 };

                    var sw = Stopwatch.StartNew();
                    var finalSol = await tabuService.RunWithSettings(baseSol.BestSchedule, settings);
                    sw.Stop();

                    // 3.6. Xuất file (Các biến copperJobsCount, aluminumJobsCount lúc này đã hợp lệ)
                    ExportToDesktop(
                        jobs.Count,
                        copperJobsCount,
                        aluminumJobsCount,
                        machines.Count,
                        combo.Tenure,
                        combo.Neighbors,
                        finalSol.BestSchedule.Makespan,
                        finalSol.BestObjective,
                        sw.Elapsed.TotalSeconds
                    );

                    Interlocked.Increment(ref totalProcessed);
                }
            });

            return $"Đã hoàn thành sinh {totalProcessed} dòng dữ liệu trên Desktop sử dụng xử lý song song!";
        }

        private void ExportToDesktop(int totalJobs, int copperJobs, int aluminumJobs, int machines, int tenure, int neighbors, double makespan, double objective, double executionTime)
        {
            lock (_fileLock)
            {
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, "Tabu_ML_Training_Data.csv");

                    string csvLine = $"{totalJobs},{copperJobs},{aluminumJobs},{machines},{tenure},{neighbors},{makespan},{objective},{executionTime}";

                    if (!File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, "TotalJobs,CopperJobs,AluminumJobs,MachineCount,Tenure,Neighbors,Makespan,Objective,ExecutionTimeSeconds" + Environment.NewLine);
                    }
                    File.AppendAllText(filePath, csvLine + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi ghi file: {ex.Message}");
                }
            }
        }
    }
}