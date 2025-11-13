using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.Dtos.Trip
{
    public class TripDetailDto
    {
        public int Id { get; set; }
        //public string TripCode { get; set; } = null!;
        //public string Purpose { get; set; } = null!;
        //public string? Notes { get; set; }
        //public DateTimeOffset ScheduledStartTime { get; set; }
        //public DateTimeOffset ScheduledEndTime { get; set; }
        public DateTimeOffset? ActualStartTime { get; set; }
        public DateTimeOffset? ActualEndTime { get; set; }
        public int? StartOdometer { get; set; }
        public int? EndOdometer { get; set; }
        //public DateTimeOffset? DispatchTime { get; set; }
        //public DateTimeOffset? ConfirmationTime { get; set; }
        public DateTimeOffset? PickUpTime { get; set; }
        //public DateTimeOffset? CancellationTime { get; set; }
        public LocationDetail FromLocation { get; set; } = new LocationDetail();
        public LocationDetail ToLocation { get; set; } = new LocationDetail();
        //public decimal Distance { get; set; }
        public TripResponseTripRequestDetailDto? TripRequest { get; set; }
        public TripResponseTripStatusDetailDto TripStatus { get; set; } = new TripResponseTripStatusDetailDto();
        public TripResponseVehicleDetailDto Vehicle { get; set; } = new TripResponseVehicleDetailDto();
        public TripResponseDriverDetailDto Driver { get; set; } = new TripResponseDriverDetailDto();
        public RejectedDetail? RejectedDetail { get; set; }
        public CancelledDetail? CancelledDetail { get; set; }
        public List<TripResponseTripExpenseDetailDto> Expenses { get; set; } = new List<TripResponseTripExpenseDetailDto>();
        //public List<FileUploadDetailDto> Attachments { get; set; } = new List<FileUploadDetailDto>();
        public decimal TotalExpense { get; set; }
        //public List<FuelLogDetailDto>? FuelLogs { get; set; } = new List<FuelLogDetailDto>();
        //public List<TripExpenseDetailDto>? TripExpenses { get; set; } = new List<TripExpenseDetailDto>();
        //public List<DriverViolationDetailDto>? DriverViolations { get; set; } = new List<DriverViolationDetailDto>();
        //public List<VehicleAccidentDetailDto>? VehicleAccidents { get; set; } = new List<VehicleAccidentDetailDto>();
        public decimal Revenue { get; set; }
        public decimal DriverSalary { get; set; }
        public int? ApprovalBy { get; set; }
        public string ApprovalByName { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    public class TripResponseTripStatusDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
    }

    public class TripResponseTripRequestDetailDto
    {
        public int Id { get; set; }
        public int TripRequestStatusId { get; set; }
        public string RequesterName { get; set; } = null!;
        public string? RequesterPhone { get; set; }
        //public DateTimeOffset RequestedAt { get; set; }
    }

    public class TripResponseVehicleDetailDto
    {
        public int Id { get; set; }
        //public string LicensePlate { get; set; } = null!;
        public string VehicleModelName { get; set; } = null!;
        public string VehicleBrandName { get; set; } = null!;
    }

    public class TripResponseDriverDetailDto
    {
        public int Id { get; set; }
        public string DriverName { get; set; } = null!;
        public string? DriverPhone { get; set; } = null!;
    }

    public class LocationDetail
    {
        public string Name { get; set; } = null!;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class RejectedDetail
    {
        public DateTimeOffset? RejectionTime { get; set; }
        public string? RejectReason { get; set; }
        public int? RejectedByUserId { get; set; }
    }

    public class CancelledDetail
    {
        public string? CancelReason { get; set; }
        public DateTimeOffset? CancellationTime { get; set; }
        public int? CancelledByUserId { get; set; }
    }

    public class TripResponseTripExpenseDetailDto
    {
        public int Id { get; set; }
        public DataItem<int> ExpenseType { get; set; } = null!;
        public decimal Amount { get; set; }
        public DetailStatusDto<int> Status { get; set; } = null!;
        //public DateTimeOffset ExpenseDate { get; set; }
        //public List<FileUploadDetailDto> Attachments { get; set; } = new List<FileUploadDetailDto>();
    }
}
