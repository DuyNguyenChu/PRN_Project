using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleRegistration;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleRegistrationController : BaseController
    {
        private readonly IVehicleRegistrationService _service;

        public VehicleRegistrationController(IVehicleRegistrationService service) => _service = service;

        [HttpGet]
        // public async Task<IActionResult> GetAll()
        // {
        //     var list = await _service.GetAllAsync();
        //     return Ok(list);
        // }

        public async Task<ApiResponse> GetAll()
        {
            var data = (await _service.GetAllAsync()).ToList();
            return ApiResponse.Success(data);
        }

        [HttpGet("search")]
        // public async Task<IActionResult> SearchVehicles(string name)
        // {
        //     try
        //     {
        //         var vehicles = await _service.SearchVehiclesAsync(name);

        //         if (!vehicles.Any())
        //             return NotFound(new { message = "Không tìm thấy xe nào phù hợp." });

        //         return Ok(vehicles);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        public async Task<ActionResult<ApiResponse>> Search([FromQuery] string name)
        {
            try
            {
                var result = await _service.SearchVehiclesAsync(name);
                return ApiResponse.Success(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        // public async Task<IActionResult> GetByID(int id)
        // {
        //     var item = await _service.GetByIdAsync(id);
        //     if (item == null) return NotFound();
        //     return Ok(item);
        // }

        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            // return Ok(result);
            return ApiResponse.Success(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] VehicleRegistrationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            // return CreatedAtAction(nameof(GetByID), new { id = created.Id }, created);
            return ApiResponse.Success(CreatedAtAction(nameof(GetById), new { id = created.Id }, created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleRegistrationUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return Ok(ok);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok(ok);
        }
    }
}