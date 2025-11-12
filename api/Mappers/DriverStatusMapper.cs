using api.Dtos.DriverStatus;
using api.Models;
using api.Helpers; 

namespace api.Mappers
{
    public static class DriverStatusMapper
    {
        public static DriverStatus ToEntity(this CreateDriverStatusDto dto)
        {
            return new DriverStatus
            {
                Color = dto.Color,
                Description = dto.Description,
                Name = dto.Name,
                CreatedDate = DateTime.Now,
                CreatedBy = dto.CreatedBy
            };
        }

        public static DriverStatus ToEntity(this UpdateDriverStatusDto dto, DriverStatus existData)
        {
            existData.Color = dto.Color;
            existData.Description = dto.Description;
            existData.Name = dto.Name;
            existData.UpdatedBy = dto.UpdatedBy;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }

        public static DriverStatusDetailDto ToDto(this DriverStatus entity)
        {
            return new DriverStatusDetailDto
            {
                Color = entity.Color,
                Description = entity.Description,
                CreatedDate = entity.CreatedDate,
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public static DriverStatusListDto ToListDto(this DriverStatus entity)
        {
            return new DriverStatusListDto
            {
                Color = entity.Color,
                Description = entity.Description,
                CreatedDate = entity.CreatedDate,
                Id = entity.Id,
                Name = entity.Name,
            };
        }
    }
}
