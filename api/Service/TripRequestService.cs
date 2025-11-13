using api.Dtos.TripRequest;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Service
{
    public class TripRequestService : ITripRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITripRequestRepository _tripRequestRepository;
        //private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        //private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        //private readonly IFireBaseService _fireBaseService;
        //private readonly IOfficeRepository _officeRepository;
        private readonly ILogger<TripRequestService> _logger;
        public TripRequestService(ITripRequestRepository tripRequestRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IUserRoleRepository userRoleRepository, ITripRepository tripRepository, ILoggerFactory loggerFactory, IDriverRepository driverRepository, IVehicleRepository vehicleRepository)
        {
            _tripRequestRepository = tripRequestRepository;
            _httpContextAccessor = httpContextAccessor;
            //_fireBaseService = fireBaseService;
            _userRepository = userRepository;
            //_userDeviceRepository = userDeviceRepository;
            _userRoleRepository = userRoleRepository;
            //_notificationRepository = notificationRepository;
            _tripRepository = tripRepository;
            _logger = loggerFactory.CreateLogger<TripRequestService>();
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            //_officeRepository = officeRepository;
        }
        public async Task<ApiResponse> CreateAsync(CreateTripRequestDto obj)
        {
            var requester = await _userRepository.GetByIdAsync(obj.CreatedBy ?? 0);
            if (requester == null)
                return ApiResponse.BadRequest();

            var model = obj.ToEntity();
            await _tripRequestRepository.BeginTransactionAsync();
            try
            {
                await _tripRequestRepository.CreateAsync(model);
                await _tripRequestRepository.SaveChangesAsync();

                //model.RequestCode = StringHelper.GenerateCode(model.Id);
                await _tripRequestRepository.UpdateAsync(model);
                await _tripRequestRepository.SaveChangesAsync();

                //var listDistpatcher = await (from a in _userRepository.GetAll()
                //                             join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
                //                             where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
                //                             select a.Id)
                //                    .ToListAsync();

                ////add noti
                //var notification = new Notification
                //{
                //    CreatedDate = DateTime.Now,
                //    Title = TripRequestMessages.Created_Title,
                //    Content = string.Format(TripRequestMessages.Created_Body, requester.FirstName + " " + requester.LastName, obj.FromLocation, obj.ToLocation),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_REQUEST,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    DirectionId = model.Id.ToString(),
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = listDistpatcher.
                //        Select(x => new UserNotification
                //        {
                //            IsRead = false,
                //            UserId = x,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        })
                //        .ToList()
                //};

                //await _notificationRepository.CreateAsync(notification);
                //await _notificationRepository.SaveChangesAsync();

                await _tripRequestRepository.EndTransactionAsync();
                //push notification
                //var listFcmToken = await _userDeviceRepository
                //    .FindByCondition(x => listDistpatcher.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
                //    .Select(x => x.DeviceToken)
                //    .ToListAsync();

                //_fireBaseService.SendNotificationAsync(listFcmToken,
                //    NotificationConstants.TripRequestMessages.Created_Title,
                //    string.Format(NotificationConstants.TripRequestMessages.Created_Body, requester.FirstName + " " + requester.LastName, obj.FromLocation, obj.ToLocation),
                //    notification.NotificationCategoryId.ToString(),
                //    model.Id.ToString()
                //    );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create trip request with message: {Message}", ex.Message);
                //await _notificationRepository.RollbackTransactionAsync();

                return ApiResponse.InternalServerError();
            }

            return ApiResponse.Created(new
            {
                model.Id,
                //Price = model.Distance > 1000 ? model.Distance * CommonConstants.Fare.REVENUE / 1000 : CommonConstants.Fare.REVENUE
            });
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateTripRequestDto> objs)
        {
            var model = objs.Select(x => x.ToEntity());

            await _tripRequestRepository.CreateListAsync(model);
            await _tripRequestRepository.SaveChangesAsync();

            return ApiResponse.Created(model.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _tripRequestRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripRequestDetailDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    //RequestCode = x.RequestCode,
                    RequesterId = x.RequesterId,
                    FromLocation = x.FromLocation,
                    FromLatitude = x.FromLatitude,
                    FromLongitude = x.FromLongtitude,
                    ToLocation = x.ToLocation,
                    ToLatitude = x.ToLatitude,
                    ToLongitude = x.ToLongtitude,
                    //RequestedAt = x.RequestedAt,
                    //ExpectedStartTime = x.ExpectedStartTime,
                    //HandledAt = x.HandledAt,
                    //Purpose = x.Purpose,
                    TripRequestStatusId = x.TripRequestStatusId,
                    //RejectReason = x.RejectReason,
                    //CancelReason = x.CancelReason
                })
                .ToListAsync();

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var data = await _tripRequestRepository
                .FindByCondition(x => x.Id == id)
                .Select(x => new TripRequestDetailDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    //RequestCode = x.RequestCode,
                    RequesterId = x.RequesterId,
                    FromLocation = x.FromLocation,
                    FromLatitude = x.FromLatitude,
                    FromLongitude = x.FromLongtitude,
                    ToLocation = x.ToLocation,
                    ToLatitude = x.ToLatitude,
                    ToLongitude = x.ToLongtitude,
                    //RequestedAt = x.RequestedAt,
                    //ExpectedStartTime = x.ExpectedStartTime,
                    //HandledAt = x.HandledAt,
                    //Purpose = x.Purpose,
                    TripRequestStatusId = x.TripRequestStatusId,
                    //RejectReason = x.RejectReason,
                    //CancelReason = x.CancelReason,
                    //Distance = x.Distance,
                    RequesterName = x.Requester.FirstName + " " + x.Requester.LastName,
                    TripRequestStatusName = x.TripRequestStatus.Name,
                    TripRequestStatusColor = x.TripRequestStatus.Color,
                    //Price = x.Distance * CommonConstants.Fare.REVENUE / 1000,
                    Trip = x.Trips.Select(trip => new TripRequestTripDto
                    {
                        TripId = trip.Id,
                        //TripCode = trip.TripCode,
                        DriverId = trip.DriverId,
                        VehicleId = trip.VehicleId,
                        //Notes = trip.Notes,
                        //ScheduledEndTime = trip.ScheduledEndTime,
                        //ScheduledStartTime = trip.ScheduledStartTime,
                        //DispatchTime = trip.DispatchTime,
                        //ConfirmationTime = trip.ConfirmationTime,
                        PickUpTime = trip.PickUpTime,
                        ActualStartTime = trip.ActualStartTime,
                        StartOdometer = trip.StartOdoMeter,
                        ActualEndTime = trip.ActualEndTime,
                        //CompletionTime = trip.CompletionTime,
                        EndOdometer = trip.EndOdoMeter,
                        //CancellationTime = trip.CancellationTime,
                        //CancelReason = trip.CancelReason,
                        TripStatusId = trip.TripStatus.Id,
                        TripStatusName = trip.TripStatus.Name,
                        TripStatusColor = trip.TripStatus.Color
                    }).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return ApiResponse.NotFound();

            if (!(HasPermission() || data.RequesterId == currentUserId))
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _tripRequestRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripRequestListDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    //RequestCode = x.RequestCode,
                    RequesterId = x.RequesterId,
                    FromLocation = x.FromLocation,
                    FromLatitude = x.FromLatitude,
                    FromLongitude = x.FromLongtitude,
                    ToLocation = x.ToLocation,
                    ToLatitude = x.ToLatitude,
                    ToLongitude = x.ToLongtitude,
                    //RequestedAt = x.RequestedAt,
                    //ExpectedStartTime = x.ExpectedStartTime,
                    //HandledAt = x.HandledAt,
                    //Purpose = x.Purpose,
                    TripRequestStatusId = x.TripRequestStatusId,
                    //RejectReason = x.RejectReason,
                    //CancelReason = x.CancelReason
                });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data
                    .Where(x =>
                    x.Description != null && x.Description.ToLower().Contains(query.Keyword.ToLower()) 
                    //x.TripRequestStatusName != null && x.TripRequestStatusName.ToLower().Contains(query.Keyword.ToLower()) ||
                    //x.Purpose != null && x.Purpose.ToLower().Contains(query.Keyword.ToLower())
                );

            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripRequestListDto>
            {
                CurrentPage = query.PageIndex,
                PageSize = query.PageSize,
                DataSource = await data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await data.CountAsync()
            };

            return ApiResponse.Success(pagedData);
        }

        //public Task<ApiResponse> GetPagedAsync<T>(AdvancedSearchQuery<T> query)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ApiResponse> GetPagedAsync(TripRequestDTParameters parameters)
        {
            var data = await _tripRequestRepository.GetPagedAsync(parameters);
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var obj = await _tripRequestRepository.GetByIdAsync(id);
            if (obj == null)
                return ApiResponse.BadRequest();

            if (obj.RequesterId != currentUserId)
                return ApiResponse.Forbidden(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                    ApiCodeConstants.Common.Forbidden
                );
            var isDeleted = await _tripRequestRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _tripRequestRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> UpdateAsync(UpdateTripRequestDto obj)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;

            var data = await _tripRequestRepository
                .FindByCondition(x => x.RequesterId == obj.RequesterId)
                .Select(x => new
                {
                    Trip = x,
                    Requester = x.Requester.FirstName + " " + x.Requester.LastName
                })
                .FirstOrDefaultAsync();
            if (data == null)
                return ApiResponse.NotFound();

            var existData = data.Trip;

            if (existData.RequesterId != currentUserId || existData.TripRequestStatusId != CommonConstants.TripRequestStatus.PENDING)
                return ApiResponse.Forbidden(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                    ApiCodeConstants.Common.Forbidden
                );


            //var notificationTitle = string.Empty;
            //var notificationContent = string.Empty;
            //if (existData.FromLocation != obj.FromLocation &&
            //    existData.ToLocation == obj.ToLocation &&
            //    existData.ExpectedStartTime == obj.ExpectedStartTime)
            //{
            //    notificationTitle = TripRequestMessages.PickupUpdated_Title;
            //    notificationContent = string.Format(TripRequestMessages.PickupUpdated_Body, data.Requester, existData.FromLocation, obj.FromLocation);
            //}
            //else if (existData.FromLocation == obj.FromLocation &&
            //    existData.ToLocation != obj.ToLocation &&
            //    existData.ExpectedStartTime == obj.ExpectedStartTime)
            //{
            //    notificationTitle = TripRequestMessages.DropoffUpdated_Title;
            //    notificationContent = string.Format(TripRequestMessages.DropoffUpdated_Body, data.Requester, existData.ToLocation, obj.ToLocation);
            //}
            //else if (existData.FromLocation == obj.FromLocation &&
            //    existData.ToLocation == obj.ToLocation &&
            //    existData.ExpectedStartTime != obj.ExpectedStartTime)
            //{
            //    notificationTitle = TripRequestMessages.DesiredTimeUpdated_Title;
            //    notificationContent = string.Format(TripRequestMessages.DesiredTimeUpdated_Body, data.Requester, existData.ToLocation, obj.ToLocation);
            //}
            //else
            //{
            //    notificationTitle = TripRequestMessages.RequestUpdated_Title;
            //    notificationContent = string.Format(TripRequestMessages.RequestUpdated_Body, data.Requester);
            //}

            obj.ToEntity(existData);

            await _tripRequestRepository.UpdateAsync(existData);
            await _tripRequestRepository.SaveChangesAsync();

            //var listDistpatcher = await (from a in _userRepository.GetAll()
            //                             join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
            //                             where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
            //                             select a.Id)
            //                    .ToListAsync();

            ////add noti
            //var notification = new Notification
            //{
            //    CreatedDate = DateTime.Now,
            //    Title = notificationTitle,
            //    Content = notificationContent,
            //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_REQUEST,
            //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
            //    DirectionId = existData.Id.ToString(),
            //    CreatedBy = CommonConstants.ADMIN_USER,
            //    UserNotifications = listDistpatcher.
            //        Select(x => new UserNotification
            //        {
            //            IsRead = false,
            //            UserId = x,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = CommonConstants.ADMIN_USER
            //        })
            //        .ToList()
            //};

            //await _notificationRepository.CreateAsync(notification);
            //await _notificationRepository.SaveChangesAsync();

            ////push notification
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => listDistpatcher.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            //_fireBaseService.SendNotificationAsync(listFcmToken,
            //    notificationTitle,
            //    notificationContent,
            //    notification.NotificationCategoryId.ToString(),
            //    existData.Id.ToString()
            //    );

            return ApiResponse.Success();
        }

        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateTripRequestDto> objs)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> GetPagedAsync(TripRequestSearchQuery query)
        {
            var data = _tripRequestRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripRequestListDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    //RequestCode = x.RequestCode,
                    RequesterId = x.RequesterId,
                    FromLocation = x.FromLocation,
                    FromLatitude = x.FromLatitude,
                    FromLongitude = x.FromLongtitude,
                    ToLocation = x.ToLocation,
                    ToLatitude = x.ToLatitude,
                    ToLongitude = x.ToLongtitude,
                    //RequestedAt = x.RequestedAt,
                    //ExpectedStartTime = x.ExpectedStartTime,
                    //HandledAt = x.HandledAt,
                    //Purpose = x.Purpose,
                    TripRequestStatusId = x.TripRequestStatusId,
                    //RejectReason = x.RejectReason,
                    //CancelReason = x.CancelReason
                });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data
                    .Where(x => 
                    x.Description != null && x.Description.ToLower().Contains(query.Keyword.ToLower())
                    //x.TripRequestStatusName != null && x.TripRequestStatusName.ToLower().Contains(query.Keyword.ToLower()) ||
                    //x.Purpose != null && x.Purpose.ToLower().Contains(query.Keyword.ToLower())
                );

            }

            if (query.TripRequestStatusId.HasValue)
                data = data
                    .Where(x => x.TripRequestStatusId == query.TripRequestStatusId);
            //if (query.CancelledByUserId.HasValue)
            //    data = data
            //        .Where(x => x.CancelledByUserId == query.CancelledByUserId);
            if (query.RequesterId.HasValue)
                data = data
                    .Where(x => x.RequesterId == query.RequesterId);

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripRequestListDto>
            {
                CurrentPage = query.PageIndex,
                PageSize = query.PageSize,
                DataSource = await data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await data.CountAsync()
            };

            return ApiResponse.Success(pagedData);
        }

        private bool HasPermission()
        {
            var currentRoleIds = _httpContextAccessor.HttpContext?.GetCurrentRoleIds();
            return currentRoleIds != null && currentRoleIds
                .Any(x => x == CommonConstants.Role.ADMIN
                    || x == CommonConstants.Role.EXECUTIVE
                    || x == CommonConstants.Role.DISPATCHER);
        }

        public async Task<ApiResponse> RejectAsync(RejectTripRequestDto obj)
        {
            var existData = await _tripRequestRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.TripRequestStatusId == CommonConstants.TripRequestStatus.PENDING);
            if (existData == null)
                return ApiResponse.BadRequest();

            obj.ToEntity(existData);
            await _tripRequestRepository.UpdateAsync(existData);
            await _tripRequestRepository.SaveChangesAsync();

            //add noti
            //var notification = new Notification
            //{
            //    CreatedDate = DateTime.Now,
            //    Title = NotificationConstants.TripRequestMessages.Rejected_Title,
            //    Content = string.Format(NotificationConstants.TripRequestMessages.Rejected_Body, existData.RequestCode, obj.RejectReason),
            //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_REQUEST,
            //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
            //    DirectionId = existData.Id.ToString(),
            //    CreatedBy = CommonConstants.ADMIN_USER,
            //    UserNotifications = new List<UserNotification>{
            //        new UserNotification
            //        {
            //            IsRead = false,
            //            UserId = existData.RequesterId,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = CommonConstants.ADMIN_USER
            //        }
            //    }
            //};

            //await _notificationRepository.CreateAsync(notification);
            //await _notificationRepository.SaveChangesAsync();

            ////push noti
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => x.UserId == existData.RequesterId && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();
            //_fireBaseService.SendNotificationAsync(
            //    listFcmToken,
            //    notification.Title,
            //    notification.Content,
            //    notification.NotificationCategoryId.ToString(),
            //    notification.DirectionId
            //);

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> CancelAsync(CancelTripRequestDto obj)
        {

            var data = await _tripRequestRepository
                .FindByCondition(x => x.RequesterId == obj.RequesterId)
                .Select(x => new
                {
                    x.Trips,
                    Requester = x.Requester.FirstName + " " + x.Requester.LastName,
                    TripRequest = x
                })
                .FirstOrDefaultAsync();
            if (data == null)
                return ApiResponse.BadRequest();

            var existData = data.TripRequest;
            var trip = data.Trips.FirstOrDefault();

            if (!(CanCancel(existData, trip) || obj.UpdatedBy == existData.RequesterId))
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);

            obj.ToEntity(existData);
            await _tripRequestRepository.UpdateAsync(existData);
            await _tripRequestRepository.SaveChangesAsync();

            //var listUserForNotification = new List<int>();
            //if (existData.TripRequestStatusId == CommonConstants.TripRequestStatus.PENDING)
            //{
            //    listUserForNotification = await (from a in _userRepository.GetAll()
            //                                     join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
            //                                     where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
            //                                     select a.Id)
            //                            .Distinct()
            //                            .ToListAsync();
            //}
            //else if (trip != null)
            //{
            //    listUserForNotification = new List<int> { trip.DriverId, trip.ApprovalBy };

            //    trip.CancellationTime = DateTime.Now;
            //    trip.TripStatusId = CommonConstants.TripStatus.CANCELLED_BY_USER;
            //    trip.CancelledByUserId = existData.RequesterId;
            //    trip.CancelReason = obj.CancelReason;
            //    trip.UpdatedBy = existData.RequesterId;
            //    trip.LastModifiedDate = DateTime.Now;

            //    await _tripRepository.UpdateAsync(trip);
            //}

            ////add noti
            //var notification = new Notification
            //{
            //    CreatedDate = DateTime.Now,
            //    Title = TripRequestMessages.CancelledByUser_Title,
            //    Content = string.Format(TripRequestMessages.CancelledByUser_Body, data.Requester, existData.FromLocation, existData.ToLocation),
            //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_REQUEST,
            //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
            //    DirectionId = existData.Id.ToString(),
            //    CreatedBy = CommonConstants.ADMIN_USER,
            //    UserNotifications = listUserForNotification
            //        .Select(x => new UserNotification
            //        {
            //            IsRead = false,
            //            UserId = x,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = CommonConstants.ADMIN_USER
            //        })
            //        .ToList()
            //};

            //await _notificationRepository.CreateAsync(notification);
            //await _notificationRepository.SaveChangesAsync();

            ////push notification
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => listUserForNotification.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            //_fireBaseService.SendNotificationAsync(listFcmToken,
            //    notification.Title,
            //    notification.Content,
            //    notification.NotificationCategoryId.ToString(),
            //    existData.Id.ToString()
            //    );

            return ApiResponse.Success();
        }
        private static bool CanCancel(TripRequest obj, Trip? trip)
        {
            if (obj.TripRequestStatusId == CommonConstants.TripRequestStatus.REJECTED ||
                obj.TripRequestStatusId == CommonConstants.TripRequestStatus.CANCELLED)
                return false;

            if (trip != null && !(trip.TripStatusId == CommonConstants.TripStatus.APPROVED ||
                trip.TripStatusId == CommonConstants.TripStatus.DRIVER_CONFIRMED ||
                trip.TripStatusId == CommonConstants.TripStatus.REJECTED_BY_DRIVER ||
                trip.TripStatusId == CommonConstants.TripStatus.DISPATCHED))

                return false;

            return true;
        }
        public async Task<ApiResponse> ApproveAsync(ApproveTripRequestDto obj)
        {
            var tripRequest = await _tripRequestRepository
                .FirstOrDefaultAsync(x => x.Id == obj.Id && x.TripRequestStatusId == CommonConstants.TripRequestStatus.PENDING);

            if (tripRequest == null)
                return ApiResponse.BadRequest();
            var driverData = await _driverRepository
                .FindByCondition(x => x.Id == obj.DriverId)
                .Select(x => new
                {
                    Driver = x,
                    UserId = x.Users.FirstOrDefault().Id,
                    FullName = x.Users.FirstOrDefault().FirstName + " " + x.Users.FirstOrDefault().LastName
                })
                .FirstOrDefaultAsync();
            if (driverData == null)
                return ApiResponse.BadRequest();

            var vehicle = await _vehicleRepository.GetByIdAsync(obj.VehicleId);
            if (vehicle == null)
                return ApiResponse.BadRequest();

            //var officeId = await _userRepository
            //    .FindByCondition(x => x.Id == tripRequest.RequesterId)
            //    //.Select(x => x.OfficeId)
            //    .FirstOrDefaultAsync();
            //if (officeId == null)
            //    return ApiResponse.BadRequest();

            obj.ToEntity(tripRequest);
            await _tripRequestRepository.BeginTransactionAsync();
            try
            {

                await _tripRequestRepository.UpdateAsync(tripRequest);

                //add trip
                var trip = obj.ToTripEntity(tripRequest);
                //trip.OfficeId = officeId;
                //trip.RegionId = vehicle.RegionId;
                await _tripRepository.CreateAsync(trip);
                await _tripRepository.SaveChangesAsync();

                //trip.TripCode = StringHelper.GenerateCode(trip.Id);
                await _tripRepository.UpdateAsync(trip);

                //update driver
                var driver = driverData.Driver;
                driver.DriverStatusId = CommonConstants.DriverStatus.ON_TRIP;
                driver.UpdatedBy = obj.ApprovalBy;
                driver.LastModifiedDate = DateTime.Now;
                await _driverRepository.UpdateAsync(driver);

                //update vehicle
                vehicle.VehicleStatusId = CommonConstants.VehicleStatus.IN_USE;
                vehicle.UpdatedBy = obj.ApprovalBy;
                vehicle.LastModifiedDate = DateTime.Now;
                _vehicleRepository.Update(vehicle);

                //add noti
                //var notificationForUser = new Notification
                //{
                //    CreatedDate = DateTime.Now,
                //    Title = TripRequestMessages.Approved_Title,
                //    Content = TripRequestMessages.Approved_Body_User,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_REQUEST,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    DirectionId = tripRequest.Id.ToString(),
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            IsRead = false,
                //            UserId = tripRequest.RequesterId,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        }
                //    }
                //};

                //var notificationForDriver = new Notification
                //{
                //    CreatedDate = DateTime.Now,
                //    Title = TripMessages.Assigned_Title_Driver,
                //    Content = string.Format(TripMessages.Assigned_Body_Driver, trip.TripCode, trip.FromLocation, trip.ToLocation, trip.ScheduledStartTime.ToString("HH:mm")),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    DirectionId = trip.Id.ToString(),
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            IsRead = false,
                //            UserId = driver.UserId,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        }
                //    }
                //};

                //await _notificationRepository.CreateListAsync(new List<Notification> { notificationForUser, notificationForDriver });
                //await _notificationRepository.SaveChangesAsync();
                await _tripRequestRepository.SaveChangesAsync();
                await _tripRequestRepository.EndTransactionAsync();

                ////push notification
                //var listFcmToken = await _userDeviceRepository
                //    .FindByCondition(x => (x.UserId == tripRequest.RequesterId || x.UserId == driver.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
                //    .GroupBy(x => x.UserId)
                //    .Select(x => new
                //    {
                //        UserId = x.Key,
                //        DeviceTokens = x
                //            .Select(xx => xx.DeviceToken)
                //            .ToList()
                //    })
                //    .ToListAsync();

                //foreach (var item in listFcmToken)
                //{
                //    if (item.UserId == tripRequest.RequesterId)
                //    {
                //        _fireBaseService.SendNotificationAsync(item.DeviceTokens,
                //            notificationForUser.Title,
                //            notificationForUser.Content,
                //            notificationForUser.NotificationCategoryId.ToString(),
                //            tripRequest.Id.ToString()
                //        );
                //    }
                //    else
                //    {
                //        _fireBaseService.SendNotificationAsync(item.DeviceTokens,
                //            notificationForDriver.Title,
                //            notificationForDriver.Content,
                //            notificationForDriver.NotificationCategoryId.ToString(),
                //            trip.Id.ToString()
                //        );
                //    }
                //}

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to approve trip request #{RequestCode} with message: {Message}", tripRequest.RequestCode, ex.Message);
                await _tripRequestRepository.RollbackTransactionAsync();

                return ApiResponse.InternalServerError();
            }

            return ApiResponse.Success();

        }
    }
}
