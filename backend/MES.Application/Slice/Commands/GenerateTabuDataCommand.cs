using MediatR;
using MesMicroservice.Algorithm.Models;
using System.Collections.Generic;

namespace MesMicroservice.Application.Commands
{
    // Lệnh này trả về đối tượng Solution, hoặc bạn có thể đổi thành string báo thành công
    public class GenerateTabuDataCommand : IRequest<Solution>
    {
        // Nhận trực tiếp mảng chuỗi từ Swagger
        public List<string> WorkOrderIds { get; set; } = new List<string>();

        // Nhận thêm tham số ép buộc để sinh data cho AI (có giá trị mặc định)
        public int TabuTenure { get; set; } = 20;
        public int NumNeighbors { get; set; } = 30;
    }
}