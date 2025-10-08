using System;
using System.Collections.Generic;

namespace api.Models;

public partial class MaintenanceRecordDetail
{
    public int Id { get; set; }

    public int RecordId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual MaintenanceRecord Record { get; set; } = null!;
}
