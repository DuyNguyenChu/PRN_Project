using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleModel;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleModelController : BaseController
    {
        private readonly IVehicleModelService _service;

        public VehicleModelController(IVehicleModelService service) => _service = service;

        [HttpGet]
        public async Task<ApiResponse> GetAll()
        {
            var data = (await _service.GetAllAsync()).ToList();
            return ApiResponse.Success(data);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse>> Search(string name)
        {
            try
            {
                var item = await _service.SearchAsync(name);

                if (!item.Any())
                    return NotFound(new { message = "Không tìm thấy xe nào phù hợp." });

                // return Ok(vehicles);
                return ApiResponse.Success(item);
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
            return ApiResponse.Success(item);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] VehicleModelCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return ApiResponse.Success(CreatedAtAction(nameof(GetByID), new { id = created.Id }, created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleModelUpdateDto dto)
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