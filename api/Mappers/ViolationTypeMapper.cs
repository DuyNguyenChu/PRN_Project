using api.Dtos.ViolationType;
using api.Models;
using api.Helpers;

namespace api.Mappers
{
    public static class ViolationTypeMapper
    {
        public static ViolationType ToEntity(this CreateViolationTypeDto obj)
        {
            return new ViolationType
            {
                CreatedBy = obj.CreatedBy,
                Description = obj.Description,
                Name = obj.Name,
                Color = obj.Color,
                CreatedDate = DateTime.Now
            };
        }

        public static ViolationType ToEntity(this UpdateViolationTypeDto obj, ViolationType existData)
        {
            existData.UpdatedBy = obj.UpdatedBy;
            existData.Description = obj.Description;
            existData.Name = obj.Name;
            existData.Color = obj.Color;
            existData.LastModifiedDate = DateTime.Now;

            return existData;
        }

        public static ViolationTypeDetailDto ToDto(this ViolationType entity)
        {
            return new ViolationTypeDetailDto
            {
                Id = entity.Id,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                Name = entity.Name,
                Color = entity.Color
            };
        }
    }
}
