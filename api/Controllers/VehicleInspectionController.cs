using api.Dtos.VehicleInspection;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers;

[Route("api/vehicle-inspections")]
[ApiController]
public class VehicleInspectionController : ControllerBase
{
    private readonly IVehicleInspectionService _service;

    public VehicleInspectionController(IVehicleInspectionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpGet("vehicle/{vehicleId:int}")]
    public async Task<IActionResult> GetByVehicleId([FromRoute] int vehicleId) => Ok(await _service.GetByVehicleIdAsync(vehicleId));

    [HttpGet("inspector/{inspectorId:int}")]
    public async Task<IActionResult> GetByInspectorId([FromRoute] int inspectorId) => Ok(await _service.GetByInspectorIdAsync(inspectorId));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q) => Ok(await _service.SearchAsync(q));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleInspectionCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] VehicleInspectionUpdateDto dto)
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