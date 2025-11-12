using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.Dtos.TripExpense
{
    public class TripExpenseDetailDto
    {
        public int Id { get; set; }
        public TripExpenseResponseVehicleDetailDto Vehicle { get; set; } = new TripExpenseResponseVehicleDetailDto();
        public TripExpenseResponseDriverDetailDto Driver { get; set; } = new TripExpenseResponseDriverDetailDto();
        public TripExpenseResponseTripDetailDto Trip { get; set; } = new TripExpenseResponseTripDetailDto();
        public DetailStatusDto<int> Status { get; set; } = new DetailStatusDto<int>();
        public DataItem<int> ExpenseType { get; set; } = new DataItem<int>();
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public int? ApprovalBy { get; set; }
        public string? ApprovalByName { get; set; }
        public DateTimeOffset? ApprovalDate { get; set; }
        public string? RejectReason { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

    }

    public class TripExpenseResponseVehicleDetailDto
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; } = null!;
    }

    public class TripExpenseResponseDriverDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class TripExpenseResponseTripDetailDto
    {
        public int Id { get; set; }
        //public string TripCode { get; set; } = null!;
    }
}
