using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using api.Dtos.DriverViolation;
using api.Dtos.FuelLog;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IDriverViolationService
    {
        Task<ApiResponse> GetPagedAsync(DriverViolationDTParameters parameters);
    }

}
