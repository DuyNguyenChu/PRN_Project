using System;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.DriverStatus;

namespace api.Controllers
{
    [Route("api/driver-status")]
    [ApiController]
    public class DriverStatusController : BaseController,
        IBaseController<int, CreateDriverStatusDto, UpdateDriverStatusDto, api.Extensions.DTParameters>
    {
        private readonly IDriverStatusService _driverStatusService;
        public DriverStatusController(IDriverStatusService driverStatusService)
        {
            _driverStatusService = driverStatusService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDriverStatusDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var data = await _driverStatusService.CreateAsync(obj);

            return BaseResult(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var data = await _driverStatusService.GetAllAsync();

            return BaseResult(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _driverStatusService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _driverStatusService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var data = await _driverStatusService.GetPagedAsync(parameters);

            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var reuslt = await _driverStatusService.SoftDeleteAsync(id);

            return BaseResult(reuslt);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateDriverStatusDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var reuslt = await _driverStatusService.UpdateAsync(obj);

            return BaseResult(reuslt);
        }

        [HttpPut("update-list")]
        public async Task<IActionResult> UpdateListAsync([FromBody] List<UpdateDriverStatusDto> objs)
        {
            var result = await _driverStatusService.UpdateListAsync(objs);

            return BaseResult(result);
        }
    }
}
