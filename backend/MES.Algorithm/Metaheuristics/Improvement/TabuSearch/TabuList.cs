namespace MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static MesMicroservice.Algorithm.Metaheuristics.Improvement.TabuSearch.Move;

    public class TabuList
    {
        // [ĐÃ SỬA] Bỏ từ khóa 'readonly' để RTS có thể tăng/giảm linh hoạt trong lúc chạy
        private int _tabuTenure;

        private readonly Queue<string> _tabuQueue;
        private readonly HashSet<string> _tabuSet;

        public TabuList(int tabuTenure)
        {
            _tabuTenure = tabuTenure;
            _tabuQueue = new Queue<string>();
            _tabuSet = new HashSet<string>();
        }

        // ========================================================
        // [MỚI] RTS: HÀM CẬP NHẬT TRÍ NHỚ (TENURE)
        // ========================================================
        public void UpdateTenure(int newTenure)
        {
            _tabuTenure = Math.Max(1, newTenure);

            while (_tabuQueue.Count > _tabuTenure)
            {
                if (_tabuQueue.Count == 0)
                    break;

                var oldMove = _tabuQueue.Dequeue();

                if (_tabuSet.Contains(oldMove))
                    _tabuSet.Remove(oldMove);
            }
        }

        public void Add(Move move)
        {
            string signature = GetSignature(move);

            // Thêm nước đi mới vào bộ nhớ
            _tabuQueue.Enqueue(signature);
            _tabuSet.Add(signature);

            // [ĐÃ SỬA] Đổi 'if' thành 'while' để đảm bảo an toàn tuyệt đối 
            // đẩy phần tử cũ nhất ra khỏi hàng đợi nếu vượt quá trí nhớ
            while (_tabuQueue.Count > _tabuTenure)
            {
                var oldSignature = _tabuQueue.Dequeue();
                _tabuSet.Remove(oldSignature);
            }
        }

        public bool IsTabu(Move move)
        {
            string signature = GetSignature(move);
            return _tabuSet.Contains(signature);
        }

        public void Clear()
        {
            _tabuQueue.Clear();
            _tabuSet.Clear();
        }

        private string GetSignature(Move move)
        {
            if (move.Type == MoveType.Insert)
            {
                return $"I_{move.JobAId}_{move.NewIndexA}";
            }

            int id1 = move.JobAId;
            int id2 = move.JobBId;

            if (id1 > id2)
            {
                return $"S_{id2}_{id1}";
            }

            return $"S_{id1}_{id2}";
        }

        public override string ToString()
        {
            return $"TabuList (Tenure: {_tabuTenure}): [{string.Join(", ", _tabuQueue)}]";
        }
    }
}