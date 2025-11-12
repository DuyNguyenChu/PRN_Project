using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.ExpenseType
{
    public class UpdateExpenseTypeDto : CreateExpenseTypeDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
