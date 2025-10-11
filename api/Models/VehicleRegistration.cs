using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VehicleRegistration : EntityBase<int>
{

    public int VehicleId { get; set; }

    public string RegistrationNumber { get; set; } = null!;

    public DateTimeOffset IssueDate { get; set; }

    public DateTimeOffset ExpiryDate { get; set; }

    public int Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
