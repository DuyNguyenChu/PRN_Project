using System;

namespace api.Dtos.VehicleAccident;


    public class VehicleAccidentUpdateDto
    {
    public int VehicleId { get; set; }
    public int DriverId { get; set; }
    public DateTimeOffset AccidentDate { get; set; }
    public string Location { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal? DamageCost { get; set; }
    public int Status { get; set; }
    public string? RejectReason { get; set; }
    }
