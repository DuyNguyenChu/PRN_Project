using api.Dtos.MaintenanceRecord;
using api.Models;
using api.Helpers;

namespace api.Mappers
{
    public static class MaintenanceRecordMapper
    {
        public static MaintenanceRecord ToMaintenanceRecordFromCreateDto(this CreateMaintenanceRecordDto obj, int creatorId)
        {
            return new MaintenanceRecord
            {
                VehicleId = obj.VehicleId,
                TripId = obj.TripId,
                Odometer = obj.Odometer,
                ServiceType = obj.ServiceType,
                ServiceProvider = obj.ServiceProvider,
                StartTime = obj.StartTime,
                EndTime = obj.EndTime ?? default, 
                Status = (int)ApprovalStatus.Pending,
                Notes = obj.Notes,
                CreatedBy = creatorId,
                CreatedDate = DateTimeOffset.Now
            };
        }

        public static void ToMaintenanceRecordFromUpdateDto(this UpdateMaintenanceRecordDto obj, MaintenanceRecord existData, int updaterId)
        {
            existData.VehicleId = obj.VehicleId;
            existData.TripId = obj.TripId;
            existData.Odometer = obj.Odometer;
            existData.ServiceType = obj.ServiceType;
            existData.ServiceProvider = obj.ServiceProvider;
            existData.StartTime = obj.StartTime;
            existData.EndTime = obj.EndTime ?? default;
            existData.Notes = obj.Notes;
            existData.UpdatedBy = updaterId; 
            existData.LastModifiedDate = DateTimeOffset.Now; 
        }

        public static void ToMaintenanceRecordFromRejectDto(this RejectMaintenanceRecordDto obj, MaintenanceRecord existData, int updaterId)
        {
            existData.RejectReason = obj.RejectReason;
            existData.UpdatedBy = updaterId;
            existData.LastModifiedDate = DateTimeOffset.Now;
            existData.ApprovedBy = updaterId; 
            existData.ApprovedDate = DateTimeOffset.Now; 
            existData.Status = (int)ApprovalStatus.Rejected; 
        }

        public static void ToMaintenanceRecordFromApprovalDto(this ApprovalMaintenanceRecordDto obj, MaintenanceRecord existData, int approverId)
        {
            existData.UpdatedBy = approverId;
            existData.LastModifiedDate = DateTimeOffset.Now;
            existData.ApprovedBy = approverId; 
            existData.ApprovedDate = DateTimeOffset.Now; 
            existData.Status = (int)ApprovalStatus.Approved; 
        }

        // Mapper cho Detail
        public static MaintenanceRecordDetail ToMaintenanceRecordDetail(this CreateMaintenanceRecordDetailDto obj, int recordId, int creatorId)
        {
            return new MaintenanceRecordDetail
            {
                RecordId = recordId, 
                Description = obj.Description, 
                Quantity = obj.Quantity, 
                UnitPrice = obj.UnitPrice, 
                TotalPrice = obj.Quantity * obj.UnitPrice, 
                CreatedBy = creatorId, 
                CreatedDate = DateTimeOffset.Now 
            };
        }
    }
}