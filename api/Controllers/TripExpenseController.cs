using System;
using api.Dtos.TripExpense;
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
    public class TripExpenseController : BaseController, IBaseController<int, CreateTripExpenseDto, UpdateTripExpenseDto, TripExpenseDTParameters>
    {
        private readonly ITripExpenseService _tripExpenseService;

        public TripExpenseController(ITripExpenseService tripExpenseService)
        {
            _tripExpenseService = tripExpenseService;
        }

        [HttpPost]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTripExpenseDto obj)
        {
            var result = await _tripExpenseService.CreateAsync(obj);
            return BaseResult(result);
        }

        [HttpPut]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTripExpenseDto obj)
        {
            var result = await _tripExpenseService.UpdateAsync(obj);
            return BaseResult(result);
        }

        [HttpPost("{tripId}/create-multiple")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateMultiAsync(int tripId, [FromBody] IEnumerable<TripCreateTripExpenseDto> obj)
        {
            var result = await _tripExpenseService.CreateListAsync(tripId, obj);
            return BaseResult(result);
        }

        [HttpGet("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.READ)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _tripExpenseService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.READ)]
        public async Task<IActionResult> GetPagedAsync([FromBody] TripExpenseDTParameters parameters)
        {
            var data = await _tripExpenseService.GetPagedAsync(parameters);
            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.DELETE)]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _tripExpenseService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _tripExpenseService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _tripExpenseService.GetAllAsync();

            return BaseResult(result);
        }

        [HttpPut("approve")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.UPDATE)]
        public async Task<IActionResult> ApproveAsync([FromBody] ApproveTripExpenseDto obj)
        {
            var result = await _tripExpenseService.ApproveAsync(obj);
            return BaseResult(result);
        }

        [HttpPut("reject")]
        [CustomAuthorize(Enums.Menu.TRIP_EXPENSE, Enums.Action.UPDATE)]
        public async Task<IActionResult> RejectAsync([FromBody] RejectTripExpenseDto obj)
        {
            var result = await _tripExpenseService.RejectAsync(obj);
            return BaseResult(result);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetApprovalStatus()
        {
            var data = await _tripExpenseService.GetStatus();
            return BaseResult(data);
        }
    }
}
