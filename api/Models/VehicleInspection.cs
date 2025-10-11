using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VehicleInspection : EntityBase<int>
{

    public int VehicleId { get; set; }

    public DateTimeOffset InspectionDate { get; set; }

    public int InspectorId { get; set; }

    public string Result { get; set; } = null!;

    public string? Notes { get; set; }

    public int Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual User Inspector { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
