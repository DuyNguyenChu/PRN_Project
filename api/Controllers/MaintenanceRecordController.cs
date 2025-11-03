using api.Interface.Services;
using api.Dtos.MaintenanceRecord; // Đổi namespace
using api.DTParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Helpers;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MaintenanceRecordController : ControllerBase
    {
        private readonly IMaintenanceRecordService _maintenanceRecordService;

        public MaintenanceRecordController(IMaintenanceRecordService maintenanceRecordService)
        {
            _maintenanceRecordService = maintenanceRecordService;
        }

        private IActionResult BaseResult(ApiResponse response)
        {
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMaintenanceRecordDto obj)
        {
            var result = await _maintenanceRecordService.CreateAsync(obj);
            return BaseResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _maintenanceRecordService.GetByIdAsync(id);
            return BaseResult(result);
        }

        [HttpPost("paged")] // Đổi tên từ "paged-advanced"
        [Authorize]
        public async Task<IActionResult> GetPagedAsync([FromBody] MaintenanceRecordDTParameters parameters)
        {
            var result = await _maintenanceRecordService.GetPagedAsync(parameters);
            return BaseResult(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateMaintenanceRecordDto obj)
        {
            var result = await _maintenanceRecordService.UpdateAsync(obj);
            return BaseResult(result);
        }

        [HttpPut("reject")]
        [Authorize]
        public async Task<IActionResult> RejectAsync([FromBody] RejectMaintenanceRecordDto obj)
        {
            var data = await _maintenanceRecordService.RejectAsync(obj);
            return BaseResult(data);
        }

        [HttpPut("{id}/approve")]
        [Authorize]
        public async Task<IActionResult> ApproveAsync(int id)
        {
            var data = await _maintenanceRecordService.ApproveAsync(id);
            return BaseResult(data);
        }

        [HttpGet("service-types")]
        public async Task<IActionResult> GetServiceTypes()
        {
            var data = await _maintenanceRecordService.GetServiceTypes();
            return BaseResult(data);
        }
    }
}