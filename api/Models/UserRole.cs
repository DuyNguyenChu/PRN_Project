using System;
using System.Collections.Generic;

namespace api.Models;

public partial class UserRole : EntityBase<int>
{

    public int RoleId { get; set; }

    public int UserId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
