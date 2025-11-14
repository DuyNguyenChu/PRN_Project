using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Driver;
using api.Dtos.FuelLog;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IDriverService : IServiceBase<int, CreateDriverDto, UpdateDriverDto, DriverDTParameters>
    {
        //Task<ApiResponse> GetAvailableDriversAsync(DriverFilter filter);
        Task<ApiResponse> GetMyProfileAsync(int userId);
        Task<ApiResponse> UpdateMyProfileAsync(int userId, UpdateDriverProfileDto profileDto);
        //Task<ApiResponse> GetTripsAsync(TripFilter filter);
        //Task<ApiResponse> GetDriverFuelLogsAsync(FuelLogSearchQuery query);
        //Task<ApiResponse> GetDriverSalariesAsync(DriverSalaryFilter filter);
        //Task<ApiResponse> GetDriverViolationsAsync(DriverViolationFilter filter);
        //Task<ApiResponse> GetTripExpensesAsync(TripExpenseFilter filter);
        //Task<ApiResponse> GetMaintenanceRecordsAsync(MaintenanceRecordFilter filter);
        Task<ApiResponse> GetLicenseClass();
        Task<ApiResponse> GetDriverAvailableAsync();
    }

}
