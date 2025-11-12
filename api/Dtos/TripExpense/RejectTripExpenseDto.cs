using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripExpense
{
    public class RejectTripExpenseDto
    {
        public int Id { get; set; }
        public string RejectReason { get; set; } = null!;
        public int? RejectBy { get; set; }
    }
}
