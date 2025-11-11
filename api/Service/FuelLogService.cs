using api.Dtos.FuelLog;
using api.DTParameters; // Sẽ được chuyển sang api.Extensions
using api.Extensions;
using api.Helpers;
using api.Interface;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api.Service
{
    public class FuelLogService : IFuelLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFuelLogRepository _fuelLogRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuelLogService(
            IUnitOfWork unitOfWork,
            IFuelLogRepository fuelLogRepository,
            ITripRepository tripRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _fuelLogRepository = fuelLogRepository;
            _tripRepository = tripRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            int currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;

            return currentUserId;
        }

        private int GetCurrentDriverId()
        {
           
            var driverIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimNames.DRIVER_ID);

            return int.TryParse(driverIdClaim?.Value, out var id) ? id : 0;
        }
       

        public async Task<ApiResponse> CreateAsync(CreateFuelLogDto obj)
        {
            int currentUserId = GetCurrentUserId();
            int currentDriverId = GetCurrentDriverId();

            if (currentDriverId == 0)
                return ApiResponse.Forbidden<string>("Người dùng không phải là lái xe."); 

            var model = obj.ToFuelLogFromCreateDto(currentUserId);

            if (obj.TripId.HasValue)
            {
                var trip = await _tripRepository.FirstOrDefaultAsync(x => x.Id == obj.TripId);
                if (trip == null || trip.DriverId != currentDriverId)
                    return ApiResponse.BadRequest<string>(null, "Chuyến đi không hợp lệ."); 

                model.DriverId = trip.DriverId;
                model.VehicleId = trip.VehicleId;
            }
            else
            {
                model.DriverId = currentDriverId;
                if (obj.VehicleId <= 0)
                    return ApiResponse.BadRequest<string>(null, "Cần cung cấp thông tin xe."); 
            }

            await _fuelLogRepository.CreateAsync(model);
            await _unitOfWork.CommitAsync(); 

            return ApiResponse.Created(model.Id); 
        }

        public async Task<ApiResponse> UpdateAsync(UpdateFuelLogDto obj)
        {
            int currentUserId = GetCurrentUserId();
            int currentDriverId = GetCurrentDriverId();
            if (currentDriverId == 0)
                return ApiResponse.Forbidden<string>("Người dùng không phải là lái xe."); 

            var existData = await _fuelLogRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending); 

            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy nhật ký nhiên liệu hoặc không thể cập nhật.");

            Console.WriteLine($"ExistData CreatedBy: {existData.CreatedBy}, CurrentUserId: {currentUserId}");
            if (currentUserId != existData.CreatedBy)
                return ApiResponse.Forbidden<string>("Bạn không có quyền cập nhật nhật ký này."); 

            if (obj.TripId.HasValue)
            {
                var trip = await _tripRepository.FirstOrDefaultAsync(x => x.Id == obj.TripId);
                if (trip == null || trip.DriverId != existData.DriverId)
                    return ApiResponse.BadRequest<string>(null, "Chuyến đi không hợp lệ.");

                existData.DriverId = trip.DriverId;
                existData.VehicleId = trip.VehicleId;
            }

            obj.ToFuelLogFromUpdateDto(existData, currentUserId);
            await _fuelLogRepository.UpdateAsync(existData);
            await _unitOfWork.CommitAsync(); 

            return ApiResponse.Success(); 
        }

        public async Task<ApiResponse> RejectAsync(RejectFuelLogDto obj)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _fuelLogRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending); 

            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy nhật ký hoặc đã được xử lý."); 

            obj.ToFuelLogFromRejectDto(existData, currentUserId);
            await _fuelLogRepository.UpdateAsync(existData);
            await _unitOfWork.CommitAsync(); 

            return ApiResponse.Success(); 
        }

        public async Task<ApiResponse> ApproveAsync(int id)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _fuelLogRepository
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == (int)ApprovalStatus.Pending); 

            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy nhật ký hoặc đã được xử lý."); 

            var dto = new ApprovalFuelLogDto { Id = id };
            dto.ToFuelLogFromApprovalDto(existData, currentUserId);
            await _fuelLogRepository.UpdateAsync(existData);
            await _unitOfWork.CommitAsync(); 

            return ApiResponse.Success(); 
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _fuelLogRepository
                .FindByCondition(fl => fl.Id == id)
                .Select(fl => new
                {
                    fl.Id,
                    Vehicle = new { Id = fl.VehicleId, Name = "[" + fl.Vehicle.RegistrationNumber + "] " + fl.Vehicle.VehicleModel.Name },
                    Driver = new { Id = fl.DriverId, Name = fl.Driver.LicenseNumber },
                    Trip = fl.TripId.HasValue ? new { Id = fl.TripId.Value, Name = fl.Trip!.Description } : null,
                    fl.Odometer,
                    fl.FuelType,
                    fl.UnitPrice,
                    fl.Quantity,
                    fl.TotalCost,
                    GasStation = fl.GasStation,
                    fl.Notes,
                    fl.Status,
                    ApprovedBy = fl.ApprovedBy.HasValue ? new { Id = fl.ApprovedBy.Value, Name = "" } : null,
                    fl.ApprovedDate,
                    fl.RejectReason,
                    fl.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return ApiResponse.NotFound(); 

            var status = CommonConstants.ApprovalStatuses.FirstOrDefault(x => x.Id == data.Status); 
            var result = new
            {
                data.Id,
                data.Vehicle,
                data.Driver,
                data.Trip,
                data.Odometer,
                data.FuelType,
                FuelTypeName = CommonConstants.GetFuelTypeName(data.FuelType), 
                data.UnitPrice,
                data.Quantity,
                data.TotalCost,
                data.GasStation,
                data.Notes,
                Status = new { Id = data.Status, Name = status?.Name, Color = status?.Color },
                data.ApprovedBy,
                data.ApprovedDate,
                data.RejectReason,
                data.CreatedDate,
                Attachments = new List<string>()
            };

            return ApiResponse.Success(result); 
        }

        public async Task<ApiResponse> GetPagedAsync(FuelLogDTParameters parameters)
        {
            var data = await _fuelLogRepository.GetPagedAsync(parameters);
            return ApiResponse.Success(data); 
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _fuelLogRepository.GetByIdAsync(id);
            if (existData == null)
                return ApiResponse.NotFound();
            if (currentUserId != existData.CreatedBy)
                return ApiResponse.Forbidden<string>("Bạn không có quyền xoá nhật ký này.");

            existData.UpdatedBy = GetCurrentUserId();
            existData.LastModifiedDate = DateTimeOffset.Now;
            await _fuelLogRepository.SoftDeleteAsync(id);

            await _unitOfWork.CommitAsync();
            return ApiResponse.Success(true); 
        }

        public async Task<ApiResponse> GetFuelTypes()
        {
            var data = CommonConstants.FuelType 
                .Select(x => new { Id = x.Key, Name = x.Value });
            return await Task.FromResult(ApiResponse.Success(data)); 
        }

        public async Task<ApiResponse> GetStatus()
        {
            var data = CommonConstants.ApprovalStatuses 
                .Select(x => new { x.Id, x.Name, x.Color });
            return await Task.FromResult(ApiResponse.Success(data)); 
        }
    }
}