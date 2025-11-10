using api.Interface.Services;
using api.Dtos.FuelLog;
using api.DTParameters; // Sẽ được chuyển sang api.Extensions
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Helpers; // Sử dụng ApiResponse mới [cite: 1654]

namespace api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FuelLogController : ControllerBase
    {
        private readonly IFuelLogService _fuelLogService;

        public FuelLogController(IFuelLogService fuelLogService)
        {
            _fuelLogService = fuelLogService;
        }

        // Helper để trả về kết quả chuẩn theo ApiResponse mới
        private IActionResult BaseResult(ApiResponse response)
        {
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFuelLogDto obj)
        {
            var result = await _fuelLogService.CreateAsync(obj);
            return BaseResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _fuelLogService.GetByIdAsync(id);
            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        [Authorize]
        public async Task<IActionResult> GetPagedAsync([FromBody] FuelLogDTParameters parameters)
        {
            var data = await _fuelLogService.GetPagedAsync(parameters);
            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _fuelLogService.SoftDeleteAsync(id);
            return BaseResult(data);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateFuelLogDto obj)
        {
            var data = await _fuelLogService.UpdateAsync(obj);
            return BaseResult(data);
        }

        [HttpPut("reject")]
        [Authorize]
        public async Task<IActionResult> RejectAsync([FromBody] RejectFuelLogDto obj)
        {
            var data = await _fuelLogService.RejectAsync(obj);
            return BaseResult(data);
        }

        [HttpPut("{id}/approve")]
        [Authorize]
        public async Task<IActionResult> ApproveAsync(int id)
        {
            var data = await _fuelLogService.ApproveAsync(id);
            return BaseResult(data);
        }

        [HttpGet("fuel-types")]
        public async Task<IActionResult> GetFuelTypes()
        {
            var data = await _fuelLogService.GetFuelTypes();
            return BaseResult(data);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetFuelLogStatus()
        {
            var data = await _fuelLogService.GetStatus();
            return BaseResult(data);
        }
    }
}