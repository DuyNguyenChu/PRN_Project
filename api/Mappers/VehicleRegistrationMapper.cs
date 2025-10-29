using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleRegistration;
using api.Models;

namespace api.Mappers
{
    public static class VehicleRegistrationMapper
    {
        public static VehicleRegistrationDto ToEntityDto(this VehicleRegistration entity)
        {
            if (entity == null) return null!;

            return new VehicleRegistrationDto
            {
                Id = entity.Id,
                VehicleId = entity.VehicleId,
                RegistrationNumber = entity.RegistrationNumber,
                IssueDate = entity.IssueDate,
                ExpiryDate = entity.ExpiryDate,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                UpdatedBy = entity.UpdatedBy,
                VehicleName = entity.Vehicle?.Name,
                IsDeleted = entity.IsDeleted
            };
        }

        // Chuyển danh sách Entity → DTO
        public static IEnumerable<VehicleRegistrationDto> ToDtoList(this IEnumerable<VehicleRegistration> entities)
        {
            return entities.Select(e => e.ToEntityDto());
        }

        // Chuyển từ CreateDto → Entity
        public static VehicleRegistration ToCreateEntity(this VehicleRegistrationCreateDto dto)
        {
            if (dto == null) return null!;

            return new VehicleRegistration
            {
                VehicleId = dto.VehicleId,
                RegistrationNumber = dto.RegistrationNumber,
                IssueDate = dto.IssueDate,
                ExpiryDate = dto.ExpiryDate,
                Status = dto.Status,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        // Chuyển từ UpdateDto → Entity (chỉ cập nhật trường được phép)
        public static VehicleRegistration ToUpdateEntity(this VehicleRegistrationUpdateDto dto, VehicleRegistration existing)
        {
            existing.VehicleId = dto.VehicleId;
            existing.RegistrationNumber = dto.RegistrationNumber;
            existing.IssueDate = dto.IssueDate;
            existing.ExpiryDate = dto.ExpiryDate;
            existing.Status = dto.Status;
            existing.LastModifiedDate = DateTimeOffset.UtcNow;
            return existing;
        }
    }
}