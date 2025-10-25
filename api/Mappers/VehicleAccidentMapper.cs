using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleAccident;
using api.Models;

namespace api.Mappers
{
    public static class VehicleAccidentMapper
    {
        public static VehicleAccidentDto ToVehicleAccidentDto(this VehicleAccident vehicleAccidentModel)
        {
            if (vehicleAccidentModel == null) return null;

            return new VehicleAccidentDto
            {
                Id = vehicleAccidentModel.Id,
                VehicleId = vehicleAccidentModel.VehicleId,
                DriverId = vehicleAccidentModel.DriverId,
                AccidentDate = vehicleAccidentModel.AccidentDate,
                Location = vehicleAccidentModel.Location,
                Description = vehicleAccidentModel.Description,
                DamageCost = vehicleAccidentModel.DamageCost,
                Status = vehicleAccidentModel.Status,
                RejectReason = vehicleAccidentModel.RejectReason,
                ApprovedBy = vehicleAccidentModel.ApprovedBy,
                ApprovedDate = vehicleAccidentModel.ApprovedDate,
                CreatedBy = vehicleAccidentModel.CreatedBy,
                CreatedDate = vehicleAccidentModel.CreatedDate,
                LastModifiedDate = vehicleAccidentModel.LastModifiedDate,
                UpdatedBy = vehicleAccidentModel.UpdatedBy
            };
        }

        public static VehicleAccident ToVehicleAccidentFromCreateDTO(this VehicleAccidentCreateDto vehicleAccidentDto)
        {
            return new VehicleAccident
            {
                VehicleId = vehicleAccidentDto.VehicleId,
                DriverId = vehicleAccidentDto.DriverId,
                AccidentDate = vehicleAccidentDto.AccidentDate,
                Location = vehicleAccidentDto.Location,
                Description = vehicleAccidentDto.Description,
                DamageCost = vehicleAccidentDto.DamageCost,
                Status = vehicleAccidentDto.Status
            };
        }

        public static VehicleAccident ToVehicleAccidentFromUpdateDTO(this VehicleAccidentUpdateDto vehicleAccidentDto, VehicleAccident existing)
        {
            {
                existing.VehicleId = vehicleAccidentDto.VehicleId;
                existing.DriverId = vehicleAccidentDto.DriverId;
                existing.AccidentDate = vehicleAccidentDto.AccidentDate;
                existing.Location = vehicleAccidentDto.Location;
                existing.Description = vehicleAccidentDto.Description;
                existing.DamageCost = vehicleAccidentDto.DamageCost;
                existing.Status = vehicleAccidentDto.Status;
                existing.RejectReason = vehicleAccidentDto.RejectReason;
                return existing;
            }
        }
    }
}