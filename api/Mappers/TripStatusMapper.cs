using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripStatus;
using api.Models;

namespace api.Mappers
{
    public static class TripStatusMapping
    {
        public static TripStatus ToEntity(this CreateTripStatusDto dto)
        {
            return new TripStatus
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
            };
        }
        public static TripStatus ToEntity(this UpdateTripStatusDto dto, TripStatus existData)
        {
            existData.Name = dto.Name;
            existData.Description = dto.Description;
            existData.Color = dto.Color;
            existData.UpdatedBy = dto.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;
            return existData;
        }
    }
}
