using System;
using System.Collections.Generic;

namespace api.Models;

public partial class UserStatus : EntityBase<int>
{

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
