using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Vehicle;
using api.Models;

namespace api.Mappers
{
    public static class VehicleMapper
    {

        public static VehicleDto ToEntityDto(this Vehicle entity)
        {
            return new VehicleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                VehicleTypeId = entity.VehicleTypeId,
                VehicleStatusId = entity.VehicleStatusId,
                VehicleBranchId = entity.VehicleBranchId,
                VehicleModelId = entity.VehicleModelId,
                RegistrationNumber = entity.RegistrationNumber,
                Color = entity.Color,
                ManufactureYear = entity.ManufactureYear,
                Description = entity.Description,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                IsDeleted = entity.IsDeleted,

                VehicleTypeName = entity.VehicleType?.Name ?? "Unknown",
                VehicleStatusName = entity.VehicleStatus?.Name ?? "Unknown",
                VehicleBranchName = entity.VehicleBranch?.Name ?? "Unknown",
                VehicleModelName = entity.VehicleModel?.Name ?? "Unknown"
            };
        }
        public static Vehicle ToCreateEntity(this VehicleCreateDto dto)
        {
            return new Vehicle
            {
                Name = dto.Name,
                VehicleTypeId = dto.VehicleTypeId,
                VehicleStatusId = dto.VehicleStatusId,
                VehicleBranchId = dto.VehicleBranchId,
                VehicleModelId = dto.VehicleModelId,
                RegistrationNumber = dto.RegistrationNumber,
                IdentificationNumber = dto.IdentificationNumber,
                EngineNumber = dto.EngineNumber,
                Color = dto.Color,
                ManufactureYear = dto.ManufactureYear,
                Description = dto.Description,
                CreatedDate = DateTimeOffset.Now,
                IsDeleted = false,
            };
        }

        public static Vehicle ToUpdateEntity(this VehicleUpdateDto dto, Vehicle existing)
        {
            existing.Name = dto.Name;
            existing.VehicleTypeId = dto.VehicleTypeId;
            existing.VehicleStatusId = dto.VehicleStatusId;
            existing.VehicleBranchId = dto.VehicleBranchId;
            existing.VehicleModelId = dto.VehicleModelId;
            existing.RegistrationNumber = dto.RegistrationNumber;
            existing.Color = dto.Color;
            existing.ManufactureYear = dto.ManufactureYear;
            existing.Description = dto.Description;
            existing.LastModifiedDate = DateTimeOffset.Now;
            existing.IsDeleted = dto.IsDeleted;
            return existing;
        }
    }
}