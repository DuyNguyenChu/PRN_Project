using System;
using System.Collections.Generic;

namespace api.Models;

public partial class MaintenanceRecord
{
    public int Id { get; set; }

    public int? TripId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public string ServiceType { get; set; } = null!;

    public string ServiceProvider { get; set; } = null!;

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public int Odometer { get; set; }

    public decimal? ServiceCost { get; set; }

    public string Notes { get; set; } = null!;

    public int Status { get; set; }

    public string? RejectReason { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual ICollection<MaintenanceRecordDetail> MaintenanceRecordDetails { get; set; } = new List<MaintenanceRecordDetail>();

    public virtual Trip? Trip { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
