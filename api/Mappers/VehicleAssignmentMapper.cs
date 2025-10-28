using System;
using api.Dtos.VehicleAssignment;
using api.Models;

namespace api.Mappers
{
    public static class VehicleAssignmentMapper
    {
        public static VehicleAssignmentDto? ToDto(this VehicleAssignment? m)
        {
            if (m == null) return null;
            return new VehicleAssignmentDto
            {
                Id = m.Id,
                VehicleId = m.VehicleId,
                DriverId = m.DriverId,
                AssignmentDate = m.AssignmentDate,
                EndDate = m.EndDate,
                Status = m.Status,
                Notes = m.Notes,
                CreatedBy = m.CreatedBy,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate,
                UpdatedBy = m.UpdatedBy
            };
        }

        public static VehicleAssignment ToModel(this VehicleAssignmentCreateDto dto)
        {
            return new VehicleAssignment
            {
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                AssignmentDate = dto.AssignmentDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        public static VehicleAssignment ApplyUpdate(this VehicleAssignment entity, VehicleAssignmentUpdateDto dto)
        {
            if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            if (dto.Notes != null) entity.Notes = dto.Notes;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            return entity;
        }
    }
}