using System;
using api.Dtos.VehicleType;
using api.Models;

namespace api.Mappers
{
    public static class VehicleTypeMapper
    {
        public static VehicleTypeDto? ToDto(this VehicleType? m)
        {
            if (m == null) return null;
            return new VehicleTypeDto
            {
                Id = m.Id,
                Name = m.Name,
                Color = m.Color,
                Description = m.Description,
                CreatedBy = m.CreatedBy,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate,
                UpdatedBy = m.UpdatedBy
            };
        }

        public static VehicleType ToModel(this VehicleTypeCreateDto dto)
        {
            return new VehicleType
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        public static VehicleType ApplyUpdate(this VehicleType entity, VehicleTypeUpdateDto dto)
        {
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Color != null) entity.Color = dto.Color;
            if (dto.Description != null) entity.Description = dto.Description;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            return entity;
        }
    }
}