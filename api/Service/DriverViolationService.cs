//using api.Dtos.DriverViolation;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Service
{
    public class DriverViolationService : IDriverViolationService
    {
        private readonly IDriverViolationRepository _driverViolationRepository;
        public DriverViolationService(IDriverViolationRepository driverViolationRepository)
        {
            _driverViolationRepository = driverViolationRepository;
        }
        public async Task<ApiResponse> GetPagedAsync(DriverViolationDTParameters parameters)
        {
            var data = await _driverViolationRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);

        }
    }

}
