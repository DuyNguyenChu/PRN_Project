using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleStatus;
using api.Models;

namespace api.Mappers
{
    public static class VehicleStatusMapper
    {
        public static VehicleStatusDto ToEntityDto(this VehicleStatus entity)
        {
            if (entity == null) return null!;
            return new VehicleStatusDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Color = entity.Color,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                UpdatedBy = entity.UpdatedBy,
                IsDeleted = entity.IsDeleted
            };
        }

        public static VehicleStatus ToCreateEntity(this VehicleStatusCreateDto dto)
        {
            if (dto == null) return null!;
            return new VehicleStatus
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTimeOffset.Now
            };
        }

        public static VehicleStatus ToUpdateEntity(this VehicleStatusUpdateDto dto, VehicleStatus entity)
        {
            if (dto == null || entity == null) return entity!;
            entity.Name = dto.Name;
            entity.Color = dto.Color;
            entity.Description = dto.Description;
            entity.UpdatedBy = dto.UpdatedBy;
            entity.LastModifiedDate = DateTimeOffset.Now;
            entity.IsDeleted = dto.IsDeleted;
            return entity;
        }
    }
}