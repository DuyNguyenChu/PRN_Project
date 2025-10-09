using System;
using System.Collections.Generic;

namespace api.Models;

public partial class DriverViolation
{
    public int Id { get; set; }

    public int ViolationTypeId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTimeOffset ViolationDate { get; set; }

    public string ViolationLocation { get; set; } = null!;

    public decimal PenaltyAmount { get; set; }

    public string ReportedBy { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;

    public virtual ViolationType ViolationType { get; set; } = null!;
}
