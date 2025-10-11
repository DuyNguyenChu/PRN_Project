using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VehicleInsurance : EntityBase<int>
{

    public int VehicleId { get; set; }

    public string InsuranceProvider { get; set; } = null!;

    public string PolicyNumber { get; set; } = null!;

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public decimal Premium { get; set; }

    public int Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
