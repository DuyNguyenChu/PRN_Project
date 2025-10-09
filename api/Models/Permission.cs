using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Permission
{
    public int Id { get; set; }

    public int MenuId { get; set; }

    public int ActionId { get; set; }

    public int RoleId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Action Action { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
