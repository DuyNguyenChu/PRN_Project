using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.ExpenseType
{
    public class CreateExpenseTypeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
    }
}
