using System;
using api.Dtos.Trip;
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
    public class
        TripController : BaseController, IBaseController<int, CreateTripDto, UpdateTripDto, TripDTParameters>
    {
        private readonly ITripService _tripService;
        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _tripService.CreateAsync(obj);

            return BaseResult(result);
        }

        [HttpGet("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.READ)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _tripService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPut]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTripDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _tripService.UpdateAsync(obj);

            return BaseResult(data);
        }


        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _tripService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.READ)]
        public async Task<IActionResult> GetPagedAsync([FromBody] TripDTParameters parameters)
        {
            var result = await _tripService.GetPagedAsync(parameters);

            return BaseResult(result);
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.DELETE)]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _tripService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPut("cancel")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> CancelAsync([FromBody] CancelTripDto obj)
        {
            var data = await _tripService.CancelAsync(obj);

            return BaseResult(data);
        }

        [HttpPut("driver/accept")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverAccept([FromBody] DriverUpdateTripDto obj)
        {
            var data = await _tripService.DriverAcceptTrip(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/reject")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverReject([FromBody] DriverRejectTripDto obj)
        {
            var data = await _tripService.DriverRejectTrip(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/moving-to-pick-up")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverMovingToPickUp([FromBody] DriverUpdateTripDto obj)
        {
            var data = await _tripService.DriverMovingToPickup(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/arrived-at-pick-up")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverArrivedAtPickUp([FromBody] DriverUpdateTripDto obj)
        {
            var data = await _tripService.DriverArrivedAtPickup(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/moving-to-destination")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverMovingToDestination([FromBody] DriverUpdateTripMovingToDestinationDto obj)
        {
            var data = await _tripService.DriverMovingToDestination(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/arrived-at-destination")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverArrivedAtDestination([FromBody] DriverUpdateTripDto obj)
        {
            var data = await _tripService.DriverArrivedAtDestination(obj);
            return BaseResult(data);
        }

        [HttpPut("driver/complete")]
        [CustomAuthorize(Enums.Menu.TRIP_LIST, Enums.Action.UPDATE)]
        public async Task<IActionResult> DriverComplete([FromBody] DriverUpdateTripCompleteDto obj)
        {
            var data = await _tripService.DriverCompleteTrip(obj);
            return BaseResult(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _tripService.GetAllAsync();

            return BaseResult(result);
        }
    }
}
