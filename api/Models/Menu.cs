using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Menu
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string MenuType { get; set; } = null!;

    public int? ParentId { get; set; }

    public string TreeIds { get; set; } = null!;

    public string? Icon { get; set; }

    public string? Url { get; set; }

    public string? ClassName { get; set; }

    public int SortOrder { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
