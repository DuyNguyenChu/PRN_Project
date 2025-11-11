using api.Dtos.MaintenanceRecord;
using api.DTParameters;
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
    public class MaintenanceRecordService : IMaintenanceRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMaintenanceRecordRepository _maintenanceRecordRepository;
        private readonly IMaintenanceRecordDetailRepository _maintenanceRecordDetailRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaintenanceRecordService(
            IUnitOfWork unitOfWork,
            IMaintenanceRecordRepository maintenanceRecordRepository,
            IMaintenanceRecordDetailRepository maintenanceRecordDetailRepository,
            ITripRepository tripRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _maintenanceRecordRepository = maintenanceRecordRepository;
            _maintenanceRecordDetailRepository = maintenanceRecordDetailRepository;
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

        public async Task<ApiResponse> CreateAsync(CreateMaintenanceRecordDto obj)
        {
            int currentUserId = GetCurrentUserId();
            int currentDriverId = GetCurrentDriverId();

            if (currentDriverId == 0)
                return ApiResponse.Forbidden<string>("Người dùng không phải là lái xe.");

             var model = obj.ToMaintenanceRecordFromCreateDto(currentUserId); 

            if (obj.TripId.HasValue)
            {
                var trip = await _tripRepository.FirstOrDefaultAsync(x => x.Id == obj.TripId);
                 if (trip == null || trip.DriverId != currentDriverId || trip.VehicleId != model.VehicleId) 
                    return ApiResponse.BadRequest<string>(null, "Chuyến đi không hợp lệ.");

                 model.DriverId = trip.DriverId; 
                 model.VehicleId = trip.VehicleId; 
            }
            else
            {
                 model.DriverId = currentDriverId; 
            }

            // Tính tổng chi phí từ chi tiết
             model.ServiceCost = obj.Details.Sum(x => x.Quantity * x.UnitPrice);

            try
            {
                await _maintenanceRecordRepository.CreateAsync(model);
                await _unitOfWork.CommitAsync(); 

                var detailModels = obj.Details
                    .Select(detail => detail.ToMaintenanceRecordDetail(model.Id, currentUserId))
                    .ToList(); 
                
                await _maintenanceRecordDetailRepository.CreateListAsync(detailModels);
                await _unitOfWork.CommitAsync();

                return ApiResponse.Created(model.Id);
            }
            catch (Exception ex)
            {
                return ApiResponse.InternalServerError();
            }
        }

        public async Task<ApiResponse> UpdateAsync(UpdateMaintenanceRecordDto obj)
        {
            int currentUserId = GetCurrentUserId();
            int currentDriverId = GetCurrentDriverId();

            if (currentDriverId == 0)
                return ApiResponse.Forbidden<string>("Người dùng không phải là lái xe.");

            var existData = await _maintenanceRecordRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending);

            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy bản ghi hoặc không thể cập nhật.");

             if (currentDriverId != existData.DriverId) 
                return ApiResponse.Forbidden<string>("Bạn không có quyền cập nhật bản ghi này.");

             obj.ToMaintenanceRecordFromUpdateDto(existData, currentUserId); 

            if (obj.TripId.HasValue)
            {
                var trip = await _tripRepository.FirstOrDefaultAsync(x => x.Id == obj.TripId);
                 if (trip == null || trip.DriverId != existData.DriverId || trip.VehicleId != obj.VehicleId) 
                    return ApiResponse.BadRequest<string>(null, "Chuyến đi không hợp lệ.");

                 existData.DriverId = trip.DriverId; 
                 existData.VehicleId = trip.VehicleId; 
            }

            try
            {
                 existData.ServiceCost = obj.Details.Sum(x => x.UnitPrice * x.Quantity); 

                 await _maintenanceRecordDetailRepository.SoftDeleteAsync(x => x.RecordId == obj.Id); 

                var detailModels = obj.Details
                    .Select(detail => detail.ToMaintenanceRecordDetail(obj.Id, currentUserId))
                    .ToList();

                await _maintenanceRecordDetailRepository.CreateListAsync(detailModels);

                // Cập nhật header
                _maintenanceRecordRepository.UpdateAsync(existData);

                await _unitOfWork.CommitAsync();

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                return ApiResponse.InternalServerError();
            }
        }

        public async Task<ApiResponse> RejectAsync(RejectMaintenanceRecordDto obj)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _maintenanceRecordRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending);

            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy bản ghi hoặc đã được xử lý.");

             obj.ToMaintenanceRecordFromRejectDto(existData, currentUserId); 
            _maintenanceRecordRepository.UpdateAsync(existData);
            await _unitOfWork.CommitAsync();

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> ApproveAsync(int id)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _maintenanceRecordRepository
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == (int)ApprovalStatus.Pending); 

            if (existData == null)
                return ApiResponse.BadRequest<string>(null,"Không tìm thấy bản ghi hoặc đã được xử lý.");

            var dto = new ApprovalMaintenanceRecordDto { Id = id };
             dto.ToMaintenanceRecordFromApprovalDto(existData, currentUserId);
            _maintenanceRecordRepository.UpdateAsync(existData);
            await _unitOfWork.CommitAsync();

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _maintenanceRecordRepository
                .FindByCondition(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    Vehicle = new { Id = x.VehicleId, Name = "[" + x.Vehicle.RegistrationNumber + "] " + x.Vehicle.VehicleModel.Name },
                    Driver = new { Id = x.DriverId, Name = x.Driver.Users.Select(u => u.FirstName + " " + u.LastName).FirstOrDefault() },
                    Trip = x.Trip != null ? new { Id = x.Trip.Id, Name = x.Trip.Description } : null,
                    x.Odometer,
                    x.ServiceType,
                    x.ServiceCost,
                    x.ServiceProvider,
                    x.StartTime,
                    x.EndTime,
                    x.Notes,
                    x.Status,
                    Approval = x.ApprovedBy.HasValue  ? new { Id = x.ApprovedBy.Value, Name = "" } : null,
                    x.ApprovedDate,
                    x.RejectReason,
                    x.CreatedDate,
                    Detail = x.MaintenanceRecordDetails
                        .Where(mrd => !mrd.IsDeleted)
                        .Select(mrd => new
                        {
                            mrd.Id,
                            mrd.Description,
                            mrd.Quantity,
                            mrd.UnitPrice,
                            mrd.TotalPrice
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return ApiResponse.NotFound();

             var status = CommonConstants.ApprovalStatuses.FirstOrDefault(x => x.Id == data.Status); 
            var serviceTypeName = CommonConstants.ServiceType.ContainsKey(data.ServiceType) ? CommonConstants.ServiceType[data.ServiceType] : "Không xác định";

            var result = new
            {
                data.Id,
                data.Vehicle,
                data.Driver,
                data.Trip,
                data.Odometer,
                ServiceType = new { Id = data.ServiceType, Name = serviceTypeName },
                data.ServiceCost,
                data.ServiceProvider,
                data.StartTime,
                data.EndTime,
                data.Notes,
                Status = new { Id = data.Status, Name = status?.Name, Color = status?.Color },
                data.Approval,
                data.ApprovedDate,
                data.RejectReason,
                data.CreatedDate,
                data.Detail,
                Attachments = new List<string>() 
            };

            return ApiResponse.Success(result);
        }

        public async Task<ApiResponse> GetPagedAsync(MaintenanceRecordDTParameters parameters)
        {
            var data = await _maintenanceRecordRepository.GetPagedAsync(parameters);
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetServiceTypes()
        {
            var data = CommonConstants.ServiceType
                .Select(x => new { Id = x.Key, Name = x.Value }); 
            return await Task.FromResult(ApiResponse.Success(data));
        }
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            int currentUserId = GetCurrentUserId();
            var existData = await _maintenanceRecordRepository
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == (int)ApprovalStatus.Pending);
            if (existData == null)
                return ApiResponse.BadRequest<string>(null, "Không tìm thấy bản ghi hoặc không thể xóa.");
             if (existData.DriverId != GetCurrentDriverId()) 
                return ApiResponse.Forbidden<string>("Bạn không có quyền xóa bản ghi này.");
            try
            {
                await _maintenanceRecordRepository.SoftDeleteAsync(id);
                await _unitOfWork.CommitAsync();
                await _maintenanceRecordDetailRepository.SoftDeleteAsync(x => x.RecordId == id);
                await _unitOfWork.CommitAsync();
                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                return ApiResponse.InternalServerError();
            }
        }
    }
}