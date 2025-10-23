using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleStatus;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleStatusController : BaseController
    {
        private readonly IVehicleStatusService _vehicleStatusService;

        public VehicleStatusController(IVehicleStatusService vehicleStatusService)
        {
            _vehicleStatusService = vehicleStatusService;
        }

        [HttpGet]
        // public async Task<ActionResult<IEnumerable<VehicleStatusDto>>> GetAll()
        // {
        //     var result = await _vehicleStatusService.GetAllAsync();
        //     return Ok(result);
        // }
        public async Task<ApiResponse> GetAll()
        {
            var data = (await _vehicleStatusService.GetAllAsync()).ToList();
            return ApiResponse.Success(data);
        }

        // GET: api/VehicleStatus/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var result = await _vehicleStatusService.GetByIdAsync(id);
            if (result == null) return NotFound();
            // return Ok(result);
            return ApiResponse.Success(result);
        }

        // GET: api/VehicleStatus/search?name=abc
        // [HttpGet("search")]
        // public async Task<ActionResult<IEnumerable<ApiResponse>>> Search([FromQuery] string name)
        // {
        //     try
        //     {
        //         var result = await _vehicleStatusService.SearchAsync(name);
        //         return Ok(result);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse>> Search([FromQuery] string name)
        {
            try
            {
                var result = await _vehicleStatusService.SearchAsync(name);
                return ApiResponse.Success(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/VehicleStatus
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] VehicleStatusCreateDto dto)
        {
            var created = await _vehicleStatusService.CreateAsync(dto);
            // return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            return ApiResponse.Success(CreatedAtAction(nameof(GetById), new { id = created.Id }, created));
        }

        // PUT: api/VehicleStatus/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleStatusUpdateDto dto)
        {
            var updated = await _vehicleStatusService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/VehicleStatus/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _vehicleStatusService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }
    }
}