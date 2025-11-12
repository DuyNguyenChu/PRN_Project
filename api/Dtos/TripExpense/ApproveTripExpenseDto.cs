using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripExpense
{
    public class ApproveTripExpenseDto
    {
        public int Id { get; set; }
        public int? ApprovedBy { get; set; }
    }
}
