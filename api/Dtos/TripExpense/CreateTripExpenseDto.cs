using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripExpense
{
    public class CreateTripExpenseDto
    {
        public int TripId { get; set; }
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public int? CreatedBy { get; set; }
    }
}
