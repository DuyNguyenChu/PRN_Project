using System;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverViolationController : BaseController
    {
        private readonly IDriverViolationService _driverViolationService;
        public DriverViolationController(IDriverViolationService driverViolationService)
        {
            _driverViolationService = driverViolationService;
        }

        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] DriverViolationDTParameters parameters)
        {
            var data = await _driverViolationService.GetPagedAsync(parameters);
            return Ok(data);
        }
    }

}
