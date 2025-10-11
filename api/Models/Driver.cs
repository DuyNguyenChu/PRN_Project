using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Driver : EntityBase<int>
{

    public string LicenseNumber { get; set; } = null!;

    public string LicenseClass { get; set; } = null!;

    public DateTimeOffset LicenseExpiryDate { get; set; }

    public int DriverStatusId { get; set; }

    public int? BankId { get; set; }

    public string? BankBranch { get; set; }

    public string? BankNumber { get; set; }

    public string? SocialInsuranceNumber { get; set; }

    public int? ExperienceYear { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public decimal BaseSalary { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual DriverStatus DriverStatus { get; set; } = null!;

    public virtual ICollection<DriverViolation> DriverViolations { get; set; } = new List<DriverViolation>();

    public virtual ICollection<FuelLog> FuelLogs { get; set; } = new List<FuelLog>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<VehicleAccident> VehicleAccidents { get; set; } = new List<VehicleAccident>();

    public virtual ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
