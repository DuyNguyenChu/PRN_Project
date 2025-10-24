using System;
using api.Dtos.VehicleInspection;
using api.Models;

namespace api.Mappers
{
    public static class VehicleInspectionMapper
    {
        public static VehicleInspectionDto? ToDto(this VehicleInspection? m)
        {
            if (m == null) return null;
            return new VehicleInspectionDto
            {
                Id = m.Id,
                VehicleId = m.VehicleId,
                InspectionDate = m.InspectionDate,
                InspectorId = m.InspectorId,
                Result = m.Result,
                Notes = m.Notes,
                Status = m.Status,
                CreatedBy = m.CreatedBy,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate,
                UpdatedBy = m.UpdatedBy
            };
        }

        public static VehicleInspection ToModel(this VehicleInspectionCreateDto dto)
        {
            return new VehicleInspection
            {
                VehicleId = dto.VehicleId,
                InspectionDate = dto.InspectionDate,
                InspectorId = dto.InspectorId,
                Result = dto.Result,
                Notes = dto.Notes,
                Status = dto.Status,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        public static VehicleInspection ApplyUpdate(this VehicleInspection entity, VehicleInspectionUpdateDto dto)
        {
            if (dto.InspectionDate.HasValue) entity.InspectionDate = dto.InspectionDate.Value;
            if (dto.InspectorId.HasValue) entity.InspectorId = dto.InspectorId.Value;
            if (dto.Result != null) entity.Result = dto.Result;
            if (dto.Notes != null) entity.Notes = dto.Notes;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            return entity;
        }
    }
}