using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleModel;
using api.Models;

namespace api.Mappers
{
    public static class VehicleModelMapper
    {
        public static VehicleModelDto ToEntityDto(this VehicleModel entity)
        {
            if (entity == null) return null!;

            return new VehicleModelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                UpdatedBy = entity.UpdatedBy,
                IsDeleted = entity.IsDeleted
            };
        }

        public static VehicleModel ToCreateEntity(this VehicleModelCreateDto dto)
        {
            return new VehicleModel
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTimeOffset.Now,
                IsDeleted = false
            };
        }

        public static VehicleModel ToUpdateEntity(this VehicleModelUpdateDto dto, VehicleModel entity)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UpdatedBy = dto.UpdatedBy;
            entity.LastModifiedDate = DateTimeOffset.Now;
            entity.IsDeleted = dto.IsDeleted;
            return entity;
        }
    }
}