using System;

namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch
{
    public class Move
    {
        // === Job A ===
        public string MoIdA { get; set; } = string.Empty;
        public int JobAId { get; set; }
        public int? StageA { get; set; }

        // === Job B ===
        public string MoIdB { get; set; } = string.Empty;
        public int JobBId { get; set; }
        public int? StageB { get; set; }

        // === Index BEFORE (Chỉ dùng để log/debug) ===
        public int OldIndexA { get; set; }
        public int OldIndexB { get; set; }

        // === Index AFTER (Chỉ dùng để log/debug) ===
        public int NewIndexA { get; set; }
        public int NewIndexB { get; set; }
        //
        public enum MoveType
        {
            Swap,
            Insert
        }
        public MoveType Type { get; set; } = MoveType.Swap;

        // [ĐÃ SỬA] ĐỊNH NGHĨA LẠI LOGIC SO SÁNH TABU
        public override bool Equals(object? obj)
        {
            if (obj is not Move m) return false;

            // Thuật toán cấm hoàn toàn việc đổi chỗ 2 Job này cho nhau một lần nữa.
            // Điều này khóa chặt bước Undo (quay lui) và chặn đứng hiện tượng lặp vòng.
            bool samePair = (this.JobAId == m.JobAId && this.JobBId == m.JobBId);
            bool reversePair = (this.JobAId == m.JobBId && this.JobBId == m.JobAId);

            return samePair || reversePair;
        }

        // [ĐÃ SỬA] LÀM NHẸ HASHCODE ĐỂ TĂNG TỐC ĐỘ RUNTIME LÊN TỐI ĐA
        public override int GetHashCode()
        {
            // HashCode giờ đây chỉ phụ thuộc vào cặp JobId, 
            // đảm bảo Dictionary/HashSet tra cứu lệnh cấm chỉ mất O(1) thời gian (Nhanh như điện)
            int minJob = Math.Min(JobAId, JobBId);
            int maxJob = Math.Max(JobAId, JobBId);

            return HashCode.Combine(minJob, maxJob);
        }

        public override string ToString()
        {
            return $"Tabu Swap: Job {JobAId} ↔ Job {JobBId}";
        }
    }
}