using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripExpense
{
    public class TripCreateTripExpenseDto
    {
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseDate { get; set; }
        public string? Notes { get; set; }
    }
}
