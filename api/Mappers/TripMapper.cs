using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Trip;
using api.Helpers;
using api.Models;

namespace api.Mappers
{
    public static class TripMapping
    {
        public static Trip ToEntity(this CreateTripDto obj)
        {
            return new Trip
            {
                //TripCode = StringHelper.GenerateCode((int)DateTime.Now.Ticks),
                TripStatusId = CommonConstants.TripStatus.DISPATCHED,
                VehicleId = obj.VehicleId,
                DriverId = obj.DriverId,
                //ScheduledStartTime = obj.ScheduledStartTime,
                //ScheduledEndTime = obj.ScheduledEndTime ?? obj.ScheduledStartTime.AddMinutes((double)(obj.Distance * CommonConstants.Fare.MinutesPerKilometer / 1000)),
                FromLocation = obj.FromLocation,
                FromLatitude = obj.FromLatitude,
                FromLongtitude = obj.FromLongitude,
                ToLocation = obj.ToLocation,
                ToLatitude = obj.ToLatitude,
                ToLongtitude = obj.ToLongitude,
                //Distance = obj.Distance,
                //Purpose = obj.Purpose,
                //Notes = obj.Notes,
                CreatedBy = obj.CreatedBy,
                CreatedDate = DateTime.Now,
                //ApprovalBy = (int)obj.CreatedBy,
                //DispatchTime = DateTime.Now,
                //RegionId = obj.RegionId ?? 0,
                //Lấy tạm doanh thu và lương theo constant * distance
                //Revenue = obj.Distance > 1000 ? CommonConstants.Fare.REVENUE * obj.Distance / 1000 : CommonConstants.Fare.REVENUE,
                //DriverSalary = obj.Distance > 1000 ? CommonConstants.Fare.DRIVER * obj.Distance / 1000 : CommonConstants.Fare.DRIVER,
            };
        }

        public static Trip ToEntity(this UpdateTripDto obj, Trip existData)
        {
            existData.VehicleId = obj.VehicleId;
            existData.DriverId = obj.DriverId;
            //existData.Purpose = obj.Purpose;
            //existData.Notes = obj.Notes;
            //existData.ScheduledStartTime = obj.ScheduledStartTime;
            //existData.ScheduledEndTime = obj.ScheduledEndTime ?? obj.ScheduledStartTime.AddMinutes((double)(obj.Distance * CommonConstants.Fare.MinutesPerKilometer / 1000));
            existData.FromLocation = obj.FromLocation;
            existData.FromLatitude = obj.FromLatitude;
            existData.FromLongtitude = obj.FromLongitude;
            existData.ToLocation = obj.ToLocation;
            existData.ToLatitude = obj.ToLatitude;
            existData.ToLongtitude = obj.ToLongitude;
            //existData.Distance = obj.Distance;
            existData.UpdatedBy = obj.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            //Lấy tạm doanh thu và lương theo constant * distance
            //existData.Revenue = obj.Distance > 1000 ? CommonConstants.Fare.REVENUE * obj.Distance / 1000 : CommonConstants.Fare.REVENUE;
            //existData.DriverSalary = obj.Distance > 1000 ? CommonConstants.Fare.DRIVER * obj.Distance / 1000 : CommonConstants.Fare.DRIVER;
            return existData;
        }
    }
}
