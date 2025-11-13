using System;
using api.Dtos.TripRequest;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripRequestController : BaseController, IBaseController<int, CreateTripRequestDto, UpdateTripRequestDto, TripRequestDTParameters>
    {
        private readonly ITripRequestService _tripRequestService;

        public TripRequestController(ITripRequestService tripRequestService)
        {
            _tripRequestService = tripRequestService;
        }

        [HttpPost]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripRequestDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _tripRequestService.CreateAsync(obj);

            return BaseResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _tripRequestService.GetAllAsync();

            return BaseResult(result);
        }

        [HttpGet("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.READ)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _tripRequestService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _tripRequestService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.READ)]
        public async Task<IActionResult> GetPagedAsync([FromBody] TripRequestDTParameters parameters)
        {
            var data = await _tripRequestService.GetPagedAsync(parameters);

            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.DELETE)]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _tripRequestService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPut]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTripRequestDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _tripRequestService.UpdateAsync(obj);

            return BaseResult(data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] TripRequestSearchQuery query)
        {
            var data = await _tripRequestService.GetPagedAsync(query);
            return BaseResult(data);
        }

        [HttpPut("reject")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> RejectAsync([FromBody] RejectTripRequestDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _tripRequestService.RejectAsync(obj);

            return BaseResult(data);
        }

        [HttpPut("approve")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> ApproveAsync([FromBody] ApproveTripRequestDto obj)
        {
            obj.ApprovalBy = this.GetLoggedInUserId();
            var data = await _tripRequestService.ApproveAsync(obj);

            return BaseResult(data);
        }

        [HttpPut("cancel")]
        [CustomAuthorize(Enums.Menu.TRIP_REQUEST_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> CancelAsync([FromBody] CancelTripRequestDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _tripRequestService.CancelAsync(obj);

            return BaseResult(data);
        }
    }
}
