using System;
using System.Collections.Generic;
using System.Linq;

namespace MesMicroservice.Algorithm.Services
{
    public class MachineTimeCalculator : IMachineTimeCalculator
    {
        public void CalculateProcessingTimes(Dictionary<string, List<Job>> jobs)
        {
            foreach (var jobList in jobs.Values)
            {
                foreach (var job in jobList)
                {
                    if (job?.EligibleMachines == null)
                        continue;

                    foreach (var machine in job.EligibleMachines)
                    {
                        CalculateProcessingTimeForMachine(job, machine);

                        // Reset thời gian cộng dồn trước khi thuật toán lập lịch chạy.
                        machine.TotalTime = 0;
                    }
                }
            }
        }

        public static double CalculateProcessTimeActualCu(Job job, Machine machine)
        {
            int parallelUnits = GetParallelUnits(machine);

            var (v, d, q, eff) =
                (machine.Properties.Speed,
                 job.Diameter,
                 job.Quatity,
                 machine.Properties.Efficiency);

            var quantityPerDay = 0.007 * d * d * v * eff * (24 * 60 * 60);

            if (quantityPerDay <= 0)
                return 48.0;

            var daysNeeded = (double)q / quantityPerDay;
            return (daysNeeded / parallelUnits) * 24.0;
        }

        public static double CalculateProcessTimeActualAl(Job job, Machine machine)
        {
            int parallelUnits = GetParallelUnits(machine);

            var v = machine.Properties.Speed > 0
                ? machine.Properties.Speed
                : job.SpeedRetirement;

            var d = job.Diameter;
            var q = job.Quatity;
            var eff = machine.Properties.Efficiency;

            // Chống trường hợp dữ liệu tốc độ bị thiếu hoặc bằng 0.
            if (v <= 0)
                v = 500;

            var quantityPerDay = 0.0021 * d * d * v * eff * (24 * 60 * 60);

            if (quantityPerDay <= 0)
                return 48.0;

            var daysNeeded = (double)q / quantityPerDay;
            return (daysNeeded / parallelUnits) * 24.0;
        }

        public double CalculateProcessingTimeForMachine(Job job, Machine machine)
        {
            if (job.ConfiguredProcessingTimeHours is double configuredHours)
            {
                if (!double.IsFinite(configuredHours) || configuredHours <= 0)
                    throw new InvalidOperationException($"Invalid configured duration for job {job.Id}.");
                machine.ProcessingTime = configuredHours;
                return configuredHours;
            }

            var type = job.Type?.Trim().ToUpperInvariant() ?? string.Empty;

            machine.ProcessingTime = type switch
            {
                "CU" => CalculateProcessTimeActualCu(job, machine),
                "AL" => CalculateProcessTimeActualAl(job, machine),
                _ => 0
            };

            return machine.ProcessingTime;
        }

        /// <summary>
        /// Lấy số đơn vị có thể chạy song song của máy.
        /// Không còn phụ thuộc ClassId EC01 / EC02 / EC03.
        /// Máy nào có Heads hoặc Lines thì tự dùng dữ liệu đó.
        /// </summary>
        private static int GetParallelUnits(Machine machine)
        {
            if (machine?.Properties == null)
                return 1;

            int heads = machine.Properties.Heads;
            int lines = machine.Properties.Lines;

            // Với dữ liệu hiện tại:
            // - máy kéo thường có Heads
            // - máy tráng thường có Lines
            // - máy kết hợp có thể có cả hai
            // Không cần biết máy thuộc EC01, EC02 hay EC03.
            int parallelUnits = Math.Max(heads, lines);

            return parallelUnits > 0 ? parallelUnits : 1;
        }

        double IMachineTimeCalculator.SetupTime(Job? from, Job? to, int stage)
        {
            // Giữ tham số stage để không phải sửa interface và các chỗ đang gọi hàm,
            // nhưng logic setup không còn phụ thuộc stage 1 / stage 2 nữa.
            _ = stage;

            if (to == null)
                return 0.0;

            var toMachines = to.EligibleMachines ?? new List<Machine>();

            if (!toMachines.Any())
                return 0.0;

            // Nếu đây là Job đầu tiên trên máy thì dùng setup lớn nhất của các máy phù hợp.
            if (from == null)
            {
                return toMachines.Max(m => m.Properties.SetupTimeMax);
            }

            /*
             * Chỉ xét setup giữa hai Job nếu chúng thực sự có ít nhất một máy chung.
             * Nhờ vậy không cần hỏi hai Job thuộc Drawing / Enameling / Both nữa.
             */
            var fromMachineCodes = (from.EligibleMachines ?? new List<Machine>())
                .Where(m => !string.IsNullOrWhiteSpace(m.Code))
                .Select(m => m.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var commonMachines = toMachines
                .Where(m => !string.IsNullOrWhiteSpace(m.Code)
                            && fromMachineCodes.Contains(m.Code))
                .ToList();

            // Không thể chạy liên tiếp trên cùng một máy => không phát sinh setup giữa hai Job này.
            if (!commonMachines.Any())
                return 0.0;

            double setupMin = commonMachines.Min(m => m.Properties.SetupTimeMin);
            double setupMax = commonMachines.Max(m => m.Properties.SetupTimeMax);

            bool sameMaterial = string.Equals(
                from.MaterialDefinitionId,
                to.MaterialDefinitionId,
                StringComparison.OrdinalIgnoreCase);

            bool sameDiameter = Math.Abs(from.Diameter - to.Diameter) < 1e-6;

            // Quy tắc setup tổng quát, không phụ thuộc tên công đoạn:
            // - cùng vật liệu + cùng đường kính: không cần đổi setup
            // - cùng vật liệu nhưng khác đường kính: setup tối thiểu
            // - khác vật liệu: setup tối đa
            if (sameMaterial && sameDiameter)
                return 0.0;

            if (sameMaterial)
                return setupMin;

            return setupMax;
        }
    }
}
