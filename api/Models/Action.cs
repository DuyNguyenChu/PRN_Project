using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Action : EntityBase<int>
{

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<ActionInMenu> ActionInMenus { get; set; } = new List<ActionInMenu>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
