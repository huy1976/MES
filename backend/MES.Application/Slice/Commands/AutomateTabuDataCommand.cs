using MediatR;
using System.Collections.Generic;

namespace MesMicroservice.Application.Commands.TabuData
{
    public class AutomateTabuDataCommand : IRequest<string>
    {
        public List<string> WorkOrderIds { get; set; } = new List<string>();

        // Cấu hình chạy Loop cho Tenure
        public int MinTenure { get; set; } = 10;
        public int MaxTenure { get; set; } = 30;
        public int TenureStep { get; set; } = 5;

        // Cấu hình chạy Loop cho Neighbors
        public int MinNeighbors { get; set; } = 20;
        public int MaxNeighbors { get; set; } = 60;
        public int NeighborsStep { get; set; } = 10;

        // Số lần chạy lại cho mỗi tổ hợp (vì Tabu có yếu tố ngẫu nhiên)
        public int RunsPerCombination { get; set; } = 1;
    }
}