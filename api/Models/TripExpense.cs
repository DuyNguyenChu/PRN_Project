using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TripExpense : EntityBase<int>
{

    public int TripId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public int ExpenseTypeId { get; set; }

    public decimal Amount { get; set; }

    public DateTimeOffset OccurenceDate { get; set; }

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

    public virtual ExpenseType ExpenseType { get; set; } = null!;

    public virtual Trip Trip { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
