using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Trip
{
    public int Id { get; set; }

    public int? TripRequestId { get; set; }

    public int VehicleId { get; set; }

    public int DriverId { get; set; }

    public int TripStatusId { get; set; }

    public DateTimeOffset EstimateStartTime { get; set; }

    public DateTimeOffset EstimateEndTime { get; set; }

    public DateTimeOffset? ActualStartTime { get; set; }

    public DateTimeOffset? ActualEndTime { get; set; }

    public DateTimeOffset? PickUpTime { get; set; }

    public int? StartOdoMeter { get; set; }

    public int? EndOdoMeter { get; set; }

    public string FromLocation { get; set; } = null!;

    public decimal? FromLatitude { get; set; }

    public decimal? FromLongtitude { get; set; }

    public string ToLocation { get; set; } = null!;

    public decimal? ToLatitude { get; set; }

    public decimal? ToLongtitude { get; set; }

    public decimal? DriverSalary { get; set; }

    public decimal? Revenue { get; set; }

    public string Description { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual ICollection<FuelLog> FuelLogs { get; set; } = new List<FuelLog>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();

    public virtual TripRequest? TripRequest { get; set; }

    public virtual TripStatus TripStatus { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
