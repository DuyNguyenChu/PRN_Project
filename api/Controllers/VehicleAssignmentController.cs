using api.Dtos.VehicleAssignment;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers;

[Route("api/vehicle-assignments")]
[ApiController]
public class VehicleAssignmentController : ControllerBase
{
    private readonly IVehicleAssignmentService _service;

    public VehicleAssignmentController(IVehicleAssignmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("vehicle/{vehicleId:int}")]
    public async Task<IActionResult> GetByVehicleId([FromRoute] int vehicleId)
    {
        var list = await _service.GetByVehicleIdAsync(vehicleId);
        return Ok(list);
    }

    [HttpGet("driver/{driverId:int}")]
    public async Task<IActionResult> GetByDriverId([FromRoute] int driverId)
    {
        var list = await _service.GetByDriverIdAsync(driverId);
        return Ok(list);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _service.SearchAsync(q);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleAssignmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] VehicleAssignmentUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _service.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
