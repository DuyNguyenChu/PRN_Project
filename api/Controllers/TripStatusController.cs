using System;
using api.Dtos.TripStatus;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripStatusController : BaseController, IBaseController<int, CreateTripStatusDto, UpdateTripStatusDto, api.Extensions.DTParameters>
    {
        private readonly ITripStatusService _tripStatusService;
        public TripStatusController(ITripStatusService tripStatusService)
        {
            _tripStatusService = tripStatusService;
        }

        [HttpPost]
        //[CustomAuthorize(Enums.Menu.TRIP_STATUS, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripStatusDto obj)
        {
            var userId = this.GetLoggedInUserId();
            obj.CreatedBy = userId;
            var result = await _tripStatusService.CreateAsync(obj);
            return BaseResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _tripStatusService.GetAllAsync();
            return BaseResult(result);
        }

        [HttpGet("{id}")]
        //[CustomAuthorize(Enums.Menu.TRIP_STATUS, Enums.Action.READ)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _tripStatusService.GetByIdAsync(id);
            return BaseResult(result);
        }


        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var result = await _tripStatusService.GetPagedAsync(query);
            return BaseResult(result);
        }

        [HttpPost("paged-advanced")]
        //[CustomAuthorize(Enums.Menu.TRIP_STATUS, Enums.Action.READ)]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var result = await _tripStatusService.GetPagedAsync(parameters);
            return BaseResult(result);
        }

        [HttpDelete("{id}")]
        //[CustomAuthorize(Enums.Menu.TRIP_STATUS, Enums.Action.DELETE)]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var result = await _tripStatusService.SoftDeleteAsync(id);
            return BaseResult(result);
        }

        [HttpPut]
        //[CustomAuthorize(Enums.Menu.TRIP_STATUS, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTripStatusDto obj)
        {
            var userId = this.GetLoggedInUserId();
            obj.UpdatedBy = userId;
            var result = await _tripStatusService.UpdateAsync(obj);
            return BaseResult(result);
        }
    }
}
