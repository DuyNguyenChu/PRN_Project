using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleBranch;
using api.Models;

namespace api.Mappers
{
    public static class VehicleBranchMapper
    {
        public static VehicleBranchDto ToEntityDto(this VehicleBranch entity)
        {
            return new VehicleBranchDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                LastModifiedDate = entity.LastModifiedDate,
                IsDeleted = entity.IsDeleted
            };
        }
        public static VehicleBranch ToCreateEntity(this VehicleBranchCreateDto dto)
        {
            return new VehicleBranch
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                LastModifiedDate = DateTimeOffset.UtcNow,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
        public static VehicleBranch ToUpdateEntity(this VehicleBranchUpdateDto dto, VehicleBranch entity)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description ?? entity.Description;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            entity.IsDeleted = dto.IsDeleted;
            return entity;
        }
    }
}