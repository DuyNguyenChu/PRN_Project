using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripRequest;
using api.Helpers;
using api.Models;

namespace api.Mappers
{
    public static class TripRequestMapping
    {
        public static TripRequest ToEntity(this CreateTripRequestDto obj)
        {
            return new TripRequest
            {
                FromLocation = obj.FromLocation,
                FromLatitude = obj.FromLatitude,
                FromLongtitude = obj.FromLongitude,
                ToLocation = obj.ToLocation,
                ToLatitude = obj.ToLatitude,
                ToLongtitude = obj.ToLongitude,
                // RequestedAt = DateTime.Now,
                // ExpectedStartTime = obj.ExpectedStartTime,
                // Purpose = obj.Purpose,
                TripRequestStatusId = CommonConstants.TripRequestStatus.PENDING,
                CreatedBy = obj.CreatedBy,
                Description = obj.Description,
                CreatedDate = DateTime.Now,
                RequesterId = obj.CreatedBy ?? 0,
                // RequestCode = DateTime.Now.Ticks.ToString(),
                // Distance = obj.Distance,
            };
        }

        public static TripRequest ToEntity(this UpdateTripRequestDto obj, TripRequest existData)
        {
            existData.FromLocation = obj.FromLocation;
            existData.FromLatitude = obj.FromLatitude;
            existData.FromLongtitude = obj.FromLongitude;
            existData.ToLocation = obj.ToLocation;
            existData.ToLatitude = obj.ToLatitude;
            existData.ToLongtitude = obj.ToLongitude;
            // existData.ExpectedStartTime = obj.ExpectedStartTime;
            // existData.Purpose = obj.Purpose;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.Description = obj.Description;
            existData.LastModifiedDate = DateTime.Now;
            // existData.Distance = obj.Distance;

            return existData;
        }

        public static TripRequest ToEntity(this RejectTripRequestDto obj, TripRequest existData)
        {
            // existData.RejectReason = obj.RejectReason;
            existData.TripRequestStatusId = CommonConstants.TripRequestStatus.REJECTED;
            // existData.CancelledByUserId = obj.UpdatedBy;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            // existData.HandledAt = DateTime.Now;

            return existData;
        }

        public static TripRequest ToEntity(this CancelTripRequestDto obj, TripRequest existData)
        {
            // existData.CancelReason = obj.CancelReason;
            existData.TripRequestStatusId = CommonConstants.TripRequestStatus.CANCELLED;
            // existData.CancelledByUserId = obj.UpdatedBy;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }
        public static TripRequestDetailDto ToDto(this TripRequest entity)
        {
            return new TripRequestDetailDto
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                // RequestCode = entity.RequestCode,
                RequesterId = entity.RequesterId,
                FromLocation = entity.FromLocation,
                FromLatitude = entity.FromLatitude,
                FromLongitude = entity.FromLongtitude,
                ToLocation = entity.ToLocation,
                ToLatitude = entity.ToLatitude,
                ToLongitude = entity.ToLongtitude,
                // RequestedAt = entity.RequestedAt,
                // ExpectedStartTime = entity.ExpectedStartTime,
                // HandledAt = entity.HandledAt,
                // Purpose = entity.Purpose,
                TripRequestStatusId = entity.TripRequestStatusId,
                // RejectReason = entity.RejectReason,
                // CancelReason = entity.CancelReason,
                // Distance = entity.Distance,
            };
        }
        public static TripRequest ToEntity(this ApproveTripRequestDto obj, TripRequest existData)
        {
            // existData.HandledAt = DateTime.Now;
            existData.TripRequestStatusId = CommonConstants.TripRequestStatus.APPROVED;
            existData.LastModifiedDate = DateTime.Now;
            existData.UpdatedBy = obj.ApprovalBy;

            return existData;
        }

        public static Trip ToTripEntity(this ApproveTripRequestDto obj, TripRequest existData)
        {
            return new Trip
            {
                // DispatchTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                CreatedBy = obj.ApprovalBy,
                // Distance = existData.Distance,
                DriverId = obj.DriverId,
                VehicleId = obj.VehicleId,
                TripStatusId = CommonConstants.TripStatus.DISPATCHED,
                // ApprovalBy = obj.ApprovalBy ?? 0,
                FromLocation = existData.FromLocation,
                FromLatitude = existData.FromLatitude,
                FromLongtitude = existData.FromLongtitude,
                ToLocation = existData.ToLocation,
                ToLatitude = existData.ToLatitude,
                ToLongtitude = existData.ToLongtitude,
                TripRequestId = existData.Id,
                // Purpose = existData.Purpose,
                // Notes = obj.Notes,
                // TripCode = DateTime.Now.Ticks.ToString(),
                // ScheduledStartTime = obj.ScheduledStartTime,
                // ScheduledEndTime = obj.ScheduledEndTime,
                // Revenue = existData.Distance > 1000 ? CommonConstants.Fare.REVENUE * existData.Distance / 1000 : CommonConstants.Fare.REVENUE,
                // DriverSalary = existData.Distance > 1000 ? CommonConstants.Fare.DRIVER * existData.Distance / 1000 : CommonConstants.Fare.DRIVER,
            };
        }
    }
}
