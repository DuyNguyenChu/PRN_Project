using System;
using System.Collections.Generic;

namespace api.Models;

public partial class FuelLog : EntityBase<int>
{

    public string GasStation { get; set; } = null!;

    public int? TripId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public string FuelType { get; set; } = null!;

    public int Odometer { get; set; }

    public decimal Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal TotalCost { get; set; }

    public string Notes { get; set; } = null!;

    public int Status { get; set; }

    public string? RejectReason { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual Trip? Trip { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
