using api.Dtos.FuelLog;
using api.Models;
using api.Helpers; // Để sử dụng ApprovalStatus

namespace api.Mappers
{
    public static class FuelLogMapper
    {
        public static FuelLog ToFuelLogFromCreateDto(this CreateFuelLogDto obj, int creatorId)
        {
            return new FuelLog
            {
                VehicleId = obj.VehicleId,
                TripId = obj.TripId,
                Odometer = obj.Odometer,
                FuelType = obj.FuelType,
                UnitPrice = obj.UnitPrice,
                Quantity = obj.Quantity, // Gán trực tiếp
                TotalCost = obj.TotalCost,
                GasStation = obj.GasStation, // Đã đổi
                Notes = obj.Notes,
                CreatedBy = creatorId,
                CreatedDate = DateTimeOffset.Now,
                Status = (int)ApprovalStatus.Pending // Gán trạng thái ban đầu
            };
        }

        public static void ToFuelLogFromUpdateDto(this UpdateFuelLogDto obj, FuelLog existData, int updaterId)
        {
            existData.VehicleId = obj.VehicleId;
            existData.TripId = obj.TripId;
            existData.Odometer = obj.Odometer;
            existData.FuelType = obj.FuelType;
            existData.UnitPrice = obj.UnitPrice;
            existData.Quantity = obj.Quantity; // Gán trực tiếp
            existData.TotalCost = obj.TotalCost;
            existData.GasStation = obj.GasStation; // Đã đổi
            existData.Notes = obj.Notes;
            existData.UpdatedBy = updaterId;
            existData.LastModifiedDate = DateTimeOffset.Now;
        }

        public static void ToFuelLogFromRejectDto(this RejectFuelLogDto obj, FuelLog existData, int updaterId)
        {
            existData.RejectReason = obj.RejectReason;
            existData.UpdatedBy = updaterId;
            existData.LastModifiedDate = DateTimeOffset.Now;
            existData.ApprovedBy = updaterId; // Đã đổi
            existData.ApprovedDate = DateTimeOffset.Now; // Đã đổi
            existData.Status = (int)ApprovalStatus.Rejected;
        }

        public static void ToFuelLogFromApprovalDto(this ApprovalFuelLogDto obj, FuelLog existData, int approverId)
        {
            existData.ApprovedBy = approverId; // Đã đổi
            existData.ApprovedDate = DateTimeOffset.Now; // Đã đổi
            existData.Status = (int)ApprovalStatus.Approved;
            existData.UpdatedBy = approverId;
            existData.LastModifiedDate = DateTimeOffset.Now;
        }
    }
}