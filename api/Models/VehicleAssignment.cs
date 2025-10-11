using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VehicleAssignment : EntityBase<int>
{

    public int VehicleId { get; set; }

    public int DriverId { get; set; }

    public DateTimeOffset AssignmentDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public int Status { get; set; }

    public string? Notes { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
