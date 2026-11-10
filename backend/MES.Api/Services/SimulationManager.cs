using MesMicroservice.Algorithm.Heuristics.InitialSchedules;
using MesMicroservice.Algorithm.Interfaces;
using MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch;
using MesMicroservice.Algorithm.Models;
using MesMicroservice.Algorithm.Models.Mapping;
using MesMicroservice.Algorithm.Services; // Thêm namespace của bạn
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MesMicroservice.Api.Services
{
    public class SimulationManager
    {
        private readonly ILogger<SimulationManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning = false;
        private CancellationTokenSource _cancellationTokenSource;

        // Đường dẫn file dữ liệu (Cùng file với Background Service đêm học)
        private readonly string _csvPath = Path.Combine(AppContext.BaseDirectory, "Tabu_Data_Ultimate.csv");

        public SimulationManager(ILogger<SimulationManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public string GetStatus() => _isRunning ? "Đang chạy cày dữ liệu (Running)" : "Đã dừng (Stopped)";

        public void StartSimulation()
        {
            if (_isRunning) return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            // Khởi chạy vòng lặp trên một luồng nền (Background Thread)
            Task.Run(() => FarmingLoop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            _logger.LogInformation("🚀 Bắt đầu cày dữ liệu AI (Phòng Tập Gym)!");
        }

        public void StopSimulation()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _logger.LogInformation("🛑 Đã dừng cày dữ liệu!");
        }
        private async Task FarmingLoop(CancellationToken token)
        {
            // Kiểm tra và tạo tiêu đề cho file CSV nếu chưa tồn tại
            if (!File.Exists(_csvPath))
            {
                string header = "TotalJobs,MachineCount,AvgEligibleMachines,MinEligibleMachines,MaxEligibleMachines,AvgProcessingTime,StdProcessingTime,AvgDiameter,DiameterVariance,AvgSetupTime,StdSetupTime,SetupConflictRatio,SameMaterialRatio,SameDiameterRatio,MachineSpeedStd,MachineEfficiencyStd,MachineCapabilityImbalance,AssignmentSensitivity,MachinePreferenceSharpness,BottleneckLoadRatio,MachineLoadStd,BottleneckSensitivity,TightDueDateRatio,AvgSlackTime,CriticalJobRatio,InitialMakespan,InitialObjective,Neighbors,Tenure";
                await File.WriteAllTextAsync(_csvPath, header + Environment.NewLine);
            }

            Random rnd = new Random();

            while (!token.IsCancellationRequested && _isRunning)
            {
                try
                {
                    // Sử dụng Scope để khởi tạo các Service từ Database
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var workOrderRepo = sp.GetRequiredService<IWorkOrderRepository>();
                        var equipmentRepo = sp.GetRequiredService<IEquipmentRepository>();
                        var materialRepo = sp.GetRequiredService<IMaterialDefinitionRepository>();
                        var prodParamRepo = sp.GetRequiredService<IProductionParameterRepository>();
                        var equipSelector = sp.GetRequiredService<IEquipmentSelectorService>();

                        // Các Service thuật toán để thực hiện Grid Search
                        var insertionMethod = sp.GetRequiredService<InsertionMethod>();
                        var tabuService = sp.GetRequiredService<TabuSearchService>();

                        // ==========================================================
                        // 1. LẤY DANH SÁCH MÁY MÓC ĐANG HOẠT ĐỘNG
                        // ==========================================================
                        var allEquipments = await equipmentRepo.GetAllAsync();
                        var equipments = allEquipments
                            .Where(e => e.EquipmentStatus != EStatus.Breakdown
                                     && e.EquipmentStatus != EStatus.Maintenance
                                     && e.EquipmentStatus != EStatus.Pause)
                            .ToList();

                        List<Machine> mockMachines = equipments.Select(eq => new Machine
                        {
                            Id = eq.Id,
                            Code = eq.ResourceId ?? eq.Id.ToString()
                        }).ToList();

                        // ==========================================================
                        // 2. TRUY VẤN DỮ LIỆU THẬT THEO DẢI LSX505 - LSX575
                        // ==========================================================
                        // Tự động tạo danh sách mã từ LSX505 đến LSX575
                        var targetCodes = Enumerable.Range(505, 71)
                                                    .Select(i => $"LSX{i}")
                                                    .ToList();

                        // Lấy dữ liệu từ Repository có sẵn
                        var fetchedWorkOrders = await workOrderRepo.GetByWorkOrderCodesAsync(targetCodes);
                        var filteredWorkOrders = fetchedWorkOrders?.ToList();

                        if (filteredWorkOrders == null || !filteredWorkOrders.Any())
                        {
                            _logger.LogWarning("⚠️ Không tìm thấy lệnh sản xuất nào trong dải LSX505 - LSX575 tại Database!");
                            await Task.Delay(10000, token); // Đợi lâu hơn nếu DB trống
                            continue;
                        }

                        // Chọn ngẫu nhiên số lượng Job từ 2 đến 80
                        int randomJobCount = rnd.Next(2, 81);
                        int maxTake = Math.Min(randomJobCount, filteredWorkOrders.Count);

                        var selectedWorkOrders = filteredWorkOrders
                            .OrderBy(x => rnd.Next())
                            .Take(maxTake)
                            .ToList();

                        // ==========================================================
                        // 3. CHUYỂN ĐỔI SANG ĐỐI TƯỢNG JOB ĐỂ CHẠY THUẬT TOÁN
                        // ==========================================================
                        List<Job> mockJobs = new List<Job>();
                        foreach (var wo in selectedWorkOrders)
                        {
                            var prodParam = await prodParamRepo.GetAsync(wo.OperationsSegment?.ProductionParameterId ?? "");
                            var prodProps = prodParam?.Properties;

                            string eqType = prodProps?.FirstOrDefault(p => p.Name.Equals("equipmentType", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString ?? "";
                            double speed = double.TryParse(prodProps?.FirstOrDefault(p => p.Name.Equals("SPEED", StringComparison.OrdinalIgnoreCase))?.Value?.ValueString, out var s) ? s : 0;

                            double inputDiam = 0;
                            if (wo.OperationsSegment?.MaterialSpecifications != null)
                            {
                                foreach (var maSpec in wo.OperationsSegment.MaterialSpecifications)
                                {
                                    var matDef = await materialRepo.GetAsync(maSpec.MaterialDefinitionId ?? "");
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
                            mockJobs.Add(WorkOrderMapper.ToJob(wo, inputDiam, eqType, speed));
                        }

                        // Gán máy móc khả thi cho từng Job
                        var allJobDict = mockJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.ToList());
                        await equipSelector.AssignEligibleMachinesToJobsAsync(allJobDict);

                        // Đồng bộ mã ResourceId cho Eligible Machines
                        foreach (var job in mockJobs)
                        {
                            if (job.EligibleMachines == null) continue;
                            foreach (var em in job.EligibleMachines)
                            {
                                var masterEq = equipments.FirstOrDefault(e => (e.ResourceId != null && e.ResourceId == em.Code) || (e.Id != null && e.Id.ToString() == em.Code));
                                if (masterEq != null) em.Code = masterEq.ResourceId ?? masterEq.Id.ToString();
                            }
                        }

                        // ==========================================================
                        // 4. GRID SEARCH: TÌM THÔNG SỐ TỐI ƯU NHẤT CHO KỊCH BẢN NÀY
                        // ==========================================================
                        var features = AIFeatureExtractor.Extract(mockJobs, mockMachines);

                        var sharedSchedule = new MesMicroservice.Algorithm.Models.Schedule(
                            mockMachines.Select(m => m.Clone()).ToList(),
                            mockJobs.GroupBy(j => j.ManufacturingOrderId).ToDictionary(g => g.Key, g => g.Select(j => j.Clone()).ToList())
                        );

                        // Tạo lời giải ban đầu bằng Insertion Method
                        var baseSolution = await insertionMethod.Run(sharedSchedule);

                        // Tập hợp các ứng viên siêu tham số để thử nghiệm
                        var candidates = new List<(int T, int N)>
                {
                    (5, 50),   (8, 80),   (11, 120), (15, 200), (20, 300)
                };

                        double bestObjective = double.MaxValue;
                        int optimalT = 10;
                        int optimalN = 20;

                        foreach (var (t, n) in candidates)
                        {
                            var testSettings = new TabuSearchSettings
                            {
                                UseAI = false,
                                TabuTenure = t,
                                NumNeighborsPerMoAndStage = n,
                                MaxIterations = 800 // Tối ưu thời gian chạy Grid Search
                            };

                            var result = await tabuService.RunWithSettings(baseSolution.BestSchedule.Clone(), testSettings);

                            if (result.BestObjective < bestObjective)
                            {
                                bestObjective = result.BestObjective;
                                optimalT = t;
                                optimalN = n;
                            }
                        }

                        // ==========================================================
                        // 5. LƯU DỮ LIỆU ĐẶC TRƯNG VÀ NHÃN TỐI ƯU VÀO CSV
                        // ==========================================================
                        string rowData = string.Join(",",
                            features.TotalJobs,
                            features.MachineCount,
                            features.AvgEligibleMachines,
                            features.MinEligibleMachines,
                            features.MaxEligibleMachines,
                            features.AvgProcessingTime,
                            features.StdProcessingTime,
                            features.AvgDiameter,
                            features.DiameterVariance,
                            features.AvgSetupTime,
                            features.StdSetupTime,
                            features.SetupConflictRatio,
                            features.SameMaterialRatio,
                            features.SameDiameterRatio,
                            features.MachineSpeedStd,
                            features.MachineEfficiencyStd,
                            features.MachineCapabilityImbalance,
                            features.AssignmentSensitivity,
                            features.MachinePreferenceSharpness,
                            features.BottleneckLoadRatio,
                            features.MachineLoadStd,
                            features.BottleneckSensitivity,
                            features.TightDueDateRatio,
                            features.AvgSlackTime,
                            features.CriticalJobRatio,
                            features.InitialMakespan,
                            features.InitialObjective,
                            optimalN,
                            optimalT
                        );

                        using (StreamWriter sw = new StreamWriter(_csvPath, append: true))
                        {
                            await sw.WriteLineAsync(rowData);
                        }

                        _logger.LogInformation($"[AI Gym] Đã học xong kịch bản {mockJobs.Count} Jobs. Kết quả tốt nhất: T={optimalT}, N={optimalN}");
                    }

                    // Khoảng nghỉ giữa các vòng lặp để bảo vệ tài nguyên hệ thống
                    await Task.Delay(2000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi phát sinh trong Simulation Farming. Sẽ khởi động lại sau 5 giây...");
                    await Task.Delay(5000, token);
                }
            }
        }


    }
}