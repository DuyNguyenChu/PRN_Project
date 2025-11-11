using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripRequestStatus;
using api.Models;

namespace api.Mappers
{
    public static class TripRequestStatusMapper
    {
        public static TripRequestStatus ToEntity(this CreateTripRequestStatusDto dto)
        {
            return new TripRequestStatus
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
            };
        }
        public static TripRequestStatus ToEntity(this UpdateTripRequestStatusDto dto, TripRequestStatus existData)
        {
            existData.Color = dto.Color;
            existData.Name = dto.Name;
            existData.Description = dto.Description;
            existData.UpdatedBy = dto.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }
    }
}
