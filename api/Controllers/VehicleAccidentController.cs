using api.Dtos.VehicleAccident;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers;

[Route("api/vehicle-accident")]
[ApiController]
public class VehicleAccidentController : ControllerBase
{
    private readonly IVehicleAccidentService _vehicleAccidentService;

    public VehicleAccidentController(IVehicleAccidentService vehicleAccidentService)
    {
        _vehicleAccidentService = vehicleAccidentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicleAccidents = await _vehicleAccidentService.GetAllAsync();
        return Ok(vehicleAccidents);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var vehicleAccident = await _vehicleAccidentService.GetByIdAsync(id);

        if (vehicleAccident == null)
        {
            return NotFound();
        }

        return Ok(vehicleAccident);
    }

    [HttpGet("vehicle/{vehicleId:int}")]
    public async Task<IActionResult> GetByVehicleId([FromRoute] int vehicleId)
    {
        var vehicleAccidents = await _vehicleAccidentService.GetByIdAsync(vehicleId);
        return Ok(vehicleAccidents);
    }

    // [HttpGet("driver/{driverId:int}")]
    // public async Task<IActionResult> GetByDriverId([FromRoute] int driverId)
    // {
    //     var vehicleAccidents = await _vehicleAccidentService.GetByDriverIdAsync(driverId);
    //     return Ok(vehicleAccidents);
    // }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleAccidentCreateDto vehicleAccidentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdAccident = await _vehicleAccidentService.CreateAsync(vehicleAccidentDto);
        return CreatedAtAction(nameof(GetById), new { id = createdAccident.Id }, createdAccident);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] VehicleAccidentUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var vehicleAccident = await _vehicleAccidentService.UpdateAsync(id, updateDto);

        if (vehicleAccident == null)
        {
            return NotFound();
        }

        return Ok(vehicleAccident);
    }

    // [HttpPut("{id:int}/approve")]
    // public async Task<IActionResult> Approve([FromRoute] int id, [FromBody] ApproveVehicleAccidentRequestDto approveDto)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     var vehicleAccident = await _vehicleAccidentService.ApproveAsync(id, approveDto);

    //     if (vehicleAccident == null)
    //     {
    //         return NotFound();
    //     }

    //     return Ok(vehicleAccident);
    // }

    // [HttpPut("{id:int}/reject")]
    // public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] RejectVehicleAccidentRequestDto rejectDto)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     var vehicleAccident = await _vehicleAccidentService.RejectAsync(id, rejectDto);

    //     if (vehicleAccident == null)
    //     {
    //         return NotFound();
    //     }

    //     return Ok(vehicleAccident);
    // }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var result = await _vehicleAccidentService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}