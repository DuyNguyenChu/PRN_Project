using System;
using api.Dtos.TripRequestStatus;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripRequestStatusController : BaseController, IBaseController<int, CreateTripRequestStatusDto, UpdateTripRequestStatusDto, api.Extensions.DTParameters>
    {
        private readonly ITripRequestStatusService _tripRequestStatusService;
        public TripRequestStatusController(ITripRequestStatusService tripRequestStatusService)
        {
            _tripRequestStatusService = tripRequestStatusService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripRequestStatusDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _tripRequestStatusService.CreateAsync(obj);

            return BaseResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _tripRequestStatusService.GetAllAsync();

            return BaseResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _tripRequestStatusService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _tripRequestStatusService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var data = await _tripRequestStatusService.GetPagedAsync(parameters);

            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _tripRequestStatusService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTripRequestStatusDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _tripRequestStatusService.UpdateAsync(obj);

            return BaseResult(data);
        }
    }
}
