using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Vehicle;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : BaseController
    {
        private readonly IVehicleService _service;

        public VehicleController(IVehicleService service) => _service = service;

        [HttpGet]
        // public async Task<IActionResult> GetAll()
        // {
        //     var list = await _service.GetAllAsync();
        //     return Ok(list);
        // }
        public async Task<IActionResult> GetAll()
        {
            var data = (await _service.GetAllAsync()).ToList();
            return BaseResult(ApiResponse.Success(data));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse>> SearchVehicles(string name)
        {
            try
            {
                var vehicles = await _service.SearchAsync(name);

                if (!vehicles.Any())
                    return NotFound(new { message = "Không tìm thấy xe nào phù hợp." });

                // return Ok(vehicles);
                return ApiResponse.Success(vehicles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetByID(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            // return Ok(item);
            return ApiResponse.Success(item);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] VehicleCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            // return CreatedAtAction(nameof(GetByID), new { id = created.Id }, created);
            return ApiResponse.Success(CreatedAtAction(nameof(GetByID), new { id = created.Id }, created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleUpdateDto dto)
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