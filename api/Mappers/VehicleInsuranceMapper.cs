using System;
using api.Dtos.VehicleInsurance;
using api.Models;

namespace api.Mappers
{
    public static class VehicleInsuranceMapper
    {
        public static VehicleInsuranceDto? ToDto(this VehicleInsurance? m)
        {
            if (m == null) return null;
            return new VehicleInsuranceDto
            {
                Id = m.Id,
                VehicleId = m.VehicleId,
                InsuranceProvider = m.InsuranceProvider,
                PolicyNumber = m.PolicyNumber,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Premium = m.Premium,
                Status = m.Status,
                CreatedBy = m.CreatedBy,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate,
                UpdatedBy = m.UpdatedBy
            };
        }

        public static VehicleInsurance ToModel(this VehicleInsuranceCreateDto dto)
        {
            return new VehicleInsurance
            {
                VehicleId = dto.VehicleId,
                InsuranceProvider = dto.InsuranceProvider,
                PolicyNumber = dto.PolicyNumber,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Premium = dto.Premium,
                Status = dto.Status,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        public static VehicleInsurance ApplyUpdate(this VehicleInsurance entity, VehicleInsuranceUpdateDto dto)
        {
            if (dto.InsuranceProvider != null) entity.InsuranceProvider = dto.InsuranceProvider;
            if (dto.PolicyNumber != null) entity.PolicyNumber = dto.PolicyNumber;
            if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
            if (dto.Premium.HasValue) entity.Premium = dto.Premium.Value;
            if (dto.Status.HasValue) entity.Status = dto.Status.Value;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            return entity;
        }
    }
}