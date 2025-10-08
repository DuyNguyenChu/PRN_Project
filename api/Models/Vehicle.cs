using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Vehicle
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int VehicleTypeId { get; set; }

    public int VehicleStatusId { get; set; }

    public int VehicleBranchId { get; set; }

    public int VehicleModelId { get; set; }

    public int? CurrentDriverId { get; set; }

    public string? RegistrationNumber { get; set; }

    public string? IdentificationNumber { get; set; }

    public string? EngineNumber { get; set; }

    public string? Color { get; set; }

    public int? ManufactureYear { get; set; }

    public string Description { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Driver? CurrentDriver { get; set; }

    public virtual ICollection<DriverViolation> DriverViolations { get; set; } = new List<DriverViolation>();

    public virtual ICollection<FuelLog> FuelLogs { get; set; } = new List<FuelLog>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public virtual ICollection<VehicleAccident> VehicleAccidents { get; set; } = new List<VehicleAccident>();

    public virtual ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();

    public virtual VehicleBranch VehicleBranch { get; set; } = null!;

    public virtual ICollection<VehicleInspection> VehicleInspections { get; set; } = new List<VehicleInspection>();

    public virtual ICollection<VehicleInsurance> VehicleInsurances { get; set; } = new List<VehicleInsurance>();

    public virtual VehicleModel VehicleModel { get; set; } = null!;

    public virtual ICollection<VehicleRegistration> VehicleRegistrations { get; set; } = new List<VehicleRegistration>();

    public virtual VehicleStatus VehicleStatus { get; set; } = null!;

    public virtual VehicleType VehicleType { get; set; } = null!;
}
