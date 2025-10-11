using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ExpenseType : EntityBase<int>
{

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();
}
