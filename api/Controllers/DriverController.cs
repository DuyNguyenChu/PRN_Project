using System;
using api.Dtos.Driver;
using api.Dtos.FuelLog;
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
    public class DriverController : BaseController
    {
        private readonly IDriverService _driverService;
        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost]
        //[CustomAuthorize(Enums.Menu.DRIVER, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDriverDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _driverService.CreateAsync(obj);
            return BaseResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _driverService.GetAllAsync();
            return BaseResult(result);
        }

        [HttpGet("{id}")]
        //[CustomAuthorize(Enums.Menu.DRIVER, Enums.Action.READ)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _driverService.GetByIdAsync(id);
            return BaseResult(result);
        }


        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var result = await _driverService.GetPagedAsync(query);
            return BaseResult(result);
        }

        [HttpPost("paged-advanced")]
        //[CustomAuthorize(Enums.Menu.DRIVER, Enums.Action.READ)]
        public async Task<IActionResult> GetPagedAsync([FromBody] DriverDTParameters parameters)
        {
            var result = await _driverService.GetPagedAsync(parameters);
            return BaseResult(result);
        }

        [HttpDelete("{id}")]
        //[CustomAuthorize(Enums.Menu.DRIVER, Enums.Action.DELETE)]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var result = await _driverService.SoftDeleteAsync(id);
            return BaseResult(result);
        }

        [HttpPut]
        //[CustomAuthorize(Enums.Menu.DRIVER, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateDriverDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var result = await _driverService.UpdateAsync(obj);
            return BaseResult(result);
        }

        //[HttpPost("available")]
        //public async Task<IActionResult> GetAvailableDriversAsync([FromBody] DriverFilter filter)
        //{
        //    var result = await _driverService.GetAvailableDriversAsync(filter);

        //    return BaseResult(result);
        //}

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var userId = this.GetLoggedInUserId();
            var result = await _driverService.GetMyProfileAsync(userId);

            return BaseResult(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateDriverProfileDto profileDto)
        {
            var userId = this.GetLoggedInUserId();
            var result = await _driverService.UpdateMyProfileAsync(userId, profileDto);

            return BaseResult(result);
        }

        //[HttpPost("me/trips")]
        //public async Task<IActionResult> GetTripsAsync([FromBody] TripFilter filter)
        //{
        //    var result = await _driverService.GetTripsAsync(filter);
        //    return BaseResult(result);
        //}

        //[HttpPost("me/fuel-logs")]
        //public async Task<IActionResult> GetDriverFuelLogsAsync([FromBody] FuelLogSearchQuery query)
        //{
        //    var result = await _driverService.GetDriverFuelLogsAsync(query);
        //    return BaseResult(result);
        //}

        //[HttpPost("me/salary")]
        //public async Task<IActionResult> GetDriverSalariesAsync([FromBody] DriverSalaryFilter filter)
        //{
        //    var result = await _driverService.GetDriverSalariesAsync(filter);
        //    return BaseResult(result);
        //}

        //[HttpPost("me/maintenance-records")]
        //public async Task<IActionResult> GetMaintenanceRecords([FromBody] MaintenanceRecordFilter filter)
        //{
        //    var result = await _driverService.GetMaintenanceRecordsAsync(filter);
        //    return BaseResult(result);
        //}

        //[HttpPost("me/trip-expenses")]
        //public async Task<IActionResult> GetTripExpenses([FromBody] TripExpenseFilter filter)
        //{
        //    var result = await _driverService.GetTripExpensesAsync(filter);
        //    return BaseResult(result);
        //}

        //[HttpPost("me/driver-violations")]
        //public async Task<IActionResult> GetDriverViolations([FromBody] DriverViolationFilter filter)
        //{
        //    var result = await _driverService.GetDriverViolationsAsync(filter);
        //    return BaseResult(result);
        //}


        [HttpGet("license-class")]
        public async Task<IActionResult> GetLicenseClass()
        {
            var result = await _driverService.GetLicenseClass();
            return BaseResult(result);
        }
    }

}
