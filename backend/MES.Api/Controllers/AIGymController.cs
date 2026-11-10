using Microsoft.AspNetCore.Mvc;
using MesMicroservice.Api.Services; // Namespace chứa SimulationManager

namespace MesMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/ai-gym")]
    public class AIGymController : ControllerBase
    {
        private readonly SimulationManager _simulationManager;

        public AIGymController(SimulationManager simulationManager)
        {
            _simulationManager = simulationManager;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { Status = _simulationManager.GetStatus() });
        }

        [HttpPost("start")]
        public IActionResult StartFarming()
        {
            _simulationManager.StartSimulation();
            return Ok(new { Message = "Đã khởi động cỗ máy cày dữ liệu AI! Hệ thống sẽ liên tục học tập." });
        }

        [HttpPost("stop")]
        public IActionResult StopFarming()
        {
            _simulationManager.StopSimulation();
            return Ok(new { Message = "Đã dừng hệ thống cày dữ liệu." });
        }
    }
}