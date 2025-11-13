using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripExpense
{
    public class TripExpenseListDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehicleLicensePlate { get; set; } = null!;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public int TripId { get; set; }
        //public string TripCode { get; set; } = null!;
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTimeOffset ExpenseDate { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; }
        public int? ApprovalBy { get; set; }
        public DateTimeOffset? ApprovalDate { get; set; }
        public string? RejectReason { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
