using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ViolationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DriverViolation> DriverViolations { get; set; } = new List<DriverViolation>();
}
