using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleBranch;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleBranchController : BaseController
    {
        private readonly IVehicleBranchService _vehicleBranchService;

        public VehicleBranchController(IVehicleBranchService vehicleBranchService)
        {
            _vehicleBranchService = vehicleBranchService;
        }

        // GET: api/VehicleBranch
        [HttpGet]
        // public async Task<ActionResult<IEnumerable<VehicleBranchDto>>> GetAll()
        // {
        //     var result = await _vehicleBranchService.GetAllAsync();
        //     return Ok(result);
        // }
        public async Task<ApiResponse> GetAll()
        {
            var data = (await _vehicleBranchService.GetAllAsync()).ToList();
            return ApiResponse.Success(data);
        }

        // GET: api/VehicleBranch/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var result = await _vehicleBranchService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/VehicleBranch/search?name=abc
        // [HttpGet("search")]
        // public async Task<ActionResult<IEnumerable<VehicleBranchDto>>> Search([FromQuery] string name)
        // {
        //     try
        //     {
        //         var result = await _vehicleBranchService.SearchAsync(name);
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
                var result = await _vehicleBranchService.SearchAsync(name);
                return ApiResponse.Success(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // POST: api/VehicleBranch
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] VehicleBranchCreateDto dto)
        {
            var created = await _vehicleBranchService.CreateAsync(dto);
            // return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            return ApiResponse.Success(CreatedAtAction(nameof(GetById), new { id = created.Id }, created));

        }

        // PUT: api/VehicleBranch/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleBranchUpdateDto dto)
        {
            var updated = await _vehicleBranchService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/VehicleBranch/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _vehicleBranchService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }
    }
}