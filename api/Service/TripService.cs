using api.Dtos.Trip;
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
    public class TripService : ITripService
    {
        private readonly ILogger<TripService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITripRepository _tripRepository;
        //private readonly INotificationRepository _notificationRepository;
        //private readonly IFireBaseService _fireBaseService;
        //private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITripRequestRepository _tripRequestRepository;
        private readonly IUserRepository _userRepository;
        //private readonly IStorageService _storageService;
        //private readonly ITripExpenseAttachmentRepository _tripExpenseAttachmentRepository;

        public TripService(ILogger<TripService> logger, IHttpContextAccessor httpContextAccessor, ITripRepository tripRepository, IDriverRepository driverRepository, IVehicleRepository vehicleRepository, ITripRequestRepository tripRequestRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tripRepository = tripRepository;
            //_notificationRepository = notificationRepository;
            //_fireBaseService = fireBaseService;
            //_userDeviceRepository = userDeviceRepository;
            _driverRepository = driverRepository;
            _vehicleRepository = vehicleRepository;
            _tripRequestRepository = tripRequestRepository;
            _userRepository = userRepository;
            //_storageService = storageService;
            //_tripExpenseAttachmentRepository = tripExpenseAttachmentRepository;
        }

        //Create trip
        public async Task<ApiResponse> CreateAsync(CreateTripDto obj)
        {
            await _tripRepository.BeginTransactionAsync();
            try
            {
                // Cập nhật Vehicle
                var vehicle = await _vehicleRepository.GetByIdAsync(obj.VehicleId);
                if (vehicle != null)
                {
                    vehicle.VehicleStatusId = CommonConstants.VehicleStatus.IN_USE;
                    vehicle.UpdatedBy = obj.CreatedBy;
                    vehicle.LastModifiedDate = DateTime.Now;
                    //Lấy region của vehicle gán vào trip
                    //obj.RegionId = vehicle.RegionId;
                    _vehicleRepository.Update(vehicle);
                }
                // Cập nhật Driver
                var driver = await _driverRepository.GetByIdAsync(obj.DriverId);
                if (driver != null)
                {
                    driver.DriverStatusId = CommonConstants.DriverStatus.ON_TRIP;
                    driver.UpdatedBy = obj.CreatedBy;
                    driver.LastModifiedDate = DateTime.Now;
                    await _driverRepository.UpdateAsync(driver);

                }

                // Tạo Trip
                var model = obj.ToEntity();
                model.Description = "";
                await _tripRepository.CreateAsync(model);
                await _tripRepository.SaveChangesAsync(); // Save để có Id sinh mã

                // Update lại tripcode trip
                //model.TripCode = StringHelper.GenerateCode(model.Id);
                //await _tripRepository.UpdateAsync(model);

                // Save cho Trip, Vehicle, Driver
                await _tripRepository.SaveChangesAsync();

                // Gửi thông báo
                //var notifications = new List<Notification>();

                //// Tài xế
                //var driverUserId = model.Driver.UserId;
                //var notiDriver = new Notification
                //{
                //    Title = NotificationConstants.TripMessages.Assigned_Title_Driver,
                //    Content = string.Format(
                //    NotificationConstants.TripMessages.Assigned_Body_Driver,
                //    model.TripCode,
                //    model.FromLocation,
                //    model.ToLocation,
                //    model.ScheduledStartTime.ToString("dd/MM/yyyy HH:mm")),
                //    DirectionId = model.Id.ToString(),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    CreatedDate = DateTime.Now,
                //    UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification { UserId = driverUserId, IsRead = false, CreatedDate = DateTime.Now, CreatedBy = CommonConstants.ADMIN_USER }
                //        }
                //};
                //notifications.Add(notiDriver);


                //await _notificationRepository.CreateListAsync(notifications);
                //await _notificationRepository.SaveChangesAsync();

                ////Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                await _tripRepository.EndTransactionAsync();
                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Trip with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Update trip
        public async Task<ApiResponse> UpdateAsync(UpdateTripDto obj)
        {
            await _tripRepository.BeginTransactionAsync();
            try
            {
                //Check xem có data không
                var existData = await _tripRepository.GetByIdAsync(obj.Id);
                if (existData == null) return ApiResponse.NotFound();

                //Chỉ cho phép cập nhật nếu chuyến đi chưa hoàn thành
                if (existData.TripStatusId != CommonConstants.TripStatus.DISPATCHED &&
                    existData.TripStatusId != CommonConstants.TripStatus.APPROVED)
                {
                    return ApiResponse.BadRequest();
                }

                // Lưu lại DriverId và VehicleId cũ
                var oldDriverId = existData.DriverId;
                var oldVehicleId = existData.VehicleId;

                // Gán dữ liệu mới vào existData
                obj.ToEntity(existData);

                bool isDriverChanged = oldDriverId != existData.DriverId;
                bool isVehicleChanged = oldVehicleId != existData.VehicleId;

                //var notifications = new List<Notification>();

                //// Nếu thay đổi Driver
                //if (isDriverChanged)
                //{
                //    // DRIVER CŨ
                //    var oldDriver = await _driverRepository.GetByIdAsync(oldDriverId);
                //    if (oldDriver != null)
                //    {
                //        oldDriver.DriverStatusId = CommonConstants.DriverStatus.AVAILABLE;
                //        oldDriver.UpdatedBy = obj.UpdatedBy;
                //        oldDriver.LastModifiedDate = DateTime.Now;
                //        await _driverRepository.UpdateAsync(oldDriver);

                //        var noti = new Notification
                //        {
                //            Title = NotificationConstants.TripMessages.DriverRemoved_Title,
                //            Content = string.Format(
                //                NotificationConstants.TripMessages.DriverRemoved_Body,
                //                existData.FromLocation,
                //                existData.ToLocation,
                //                existData.ScheduledStartTime.ToString("dd/MM/yyyy HH:mm")
                //            ),
                //            DirectionId = existData.Id.ToString(),
                //            NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //            NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //            CreatedBy = CommonConstants.ADMIN_USER,
                //            UserNotifications = new List<UserNotification>
                //            {
                //                new UserNotification
                //                {
                //                    UserId = oldDriver.UserId,
                //                    IsRead = false,
                //                    CreatedDate = DateTime.Now,
                //                    CreatedBy = CommonConstants.ADMIN_USER
                //                }
                //            },
                //            CreatedDate = DateTime.Now
                //        };
                //        notifications.Add(noti);
                //    }

                //    // DRIVER MỚI
                //    var newDriver = await _driverRepository.GetByIdAsync(existData.DriverId);
                //    if (newDriver != null)
                //    {
                //        newDriver.DriverStatusId = CommonConstants.DriverStatus.ON_TRIP;
                //        newDriver.UpdatedBy = obj.UpdatedBy;
                //        newDriver.LastModifiedDate = DateTime.Now;
                //        await _driverRepository.UpdateAsync(newDriver);

                //        var noti = new Notification
                //        {
                //            Title = NotificationConstants.TripMessages.Assigned_Title_Driver,
                //            Content = string.Format(
                //                    NotificationConstants.TripMessages.Assigned_Body_Driver,
                //                    existData.TripCode,
                //                    existData.FromLocation,
                //                    existData.ToLocation,
                //                    existData.ScheduledStartTime.ToString("dd/MM/yyyy HH:mm")
                //                ),
                //            DirectionId = existData.Id.ToString(),
                //            NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //            NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //            CreatedBy = CommonConstants.ADMIN_USER,
                //            UserNotifications = new List<UserNotification>
                //            {
                //                new UserNotification
                //                {
                //                    UserId = newDriver.UserId,
                //                    IsRead = false,
                //                    CreatedDate = DateTime.Now,
                //                    CreatedBy = CommonConstants.ADMIN_USER

                //                }
                //            },
                //            CreatedDate = DateTime.Now
                //        };
                //        notifications.Add(noti);
                //    }
                //}

                // Nếu thay đổi Vehicle
                if (isVehicleChanged)
                {
                    // Xe cũ
                    var oldVehicle = await _vehicleRepository.GetByIdAsync(oldVehicleId);
                    if (oldVehicle != null)
                    {
                        oldVehicle.VehicleStatusId = CommonConstants.VehicleStatus.AVAILABLE;
                        oldVehicle.UpdatedBy = obj.UpdatedBy;
                        oldVehicle.LastModifiedDate = DateTime.Now;
                        _vehicleRepository.Update(oldVehicle);
                    }

                    // Xe mới
                    var newVehicle = await _vehicleRepository.GetByIdAsync(existData.VehicleId);
                    if (newVehicle != null)
                    {
                        newVehicle.VehicleStatusId = CommonConstants.VehicleStatus.IN_USE;
                        newVehicle.UpdatedBy = obj.UpdatedBy;
                        newVehicle.LastModifiedDate = DateTime.Now;

                        _vehicleRepository.Update(newVehicle);

                        //Cập nhật khu vực nếu update xe
                        //existData.RegionId = newVehicle.RegionId;
                    }
                }

                // Cập nhật chuyến đi
                await _tripRepository.UpdateAsync(existData);
                // Lưu thông báo nếu có
                //if (notifications.Any())
                //{
                //    await _notificationRepository.CreateListAsync(notifications);
                //}
                await _tripRepository.SaveChangesAsync();
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Trip with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Delete Trip
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _tripRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _tripRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        //Detail trip
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _tripRepository
                  .FindByCondition(x => !x.IsDeleted && x.Id == id)
                  .Select(x => new TripDetailDto()
                  {
                      Id = x.Id,
                      //TripCode = x.TripCode,
                      //Purpose = x.Purpose,
                      //Notes = x.Notes,
                      //ScheduledStartTime = x.ScheduledStartTime,
                      //ScheduledEndTime = x.ScheduledEndTime,
                      ActualStartTime = x.ActualStartTime,
                      ActualEndTime = x.ActualEndTime,
                      StartOdometer = x.StartOdoMeter,
                      EndOdometer = x.EndOdoMeter,
                      //DispatchTime = x.DispatchTime,
                      //ConfirmationTime = x.ConfirmationTime,
                      PickUpTime = x.PickUpTime,
                      //CancellationTime = x.CancellationTime,
                      FromLocation = new LocationDetail()
                      {
                          Name = x.FromLocation,
                          Latitude = x.FromLatitude,
                          Longitude = x.FromLongtitude
                      },
                      ToLocation = new LocationDetail()
                      {
                          Name = x.ToLocation,
                          Latitude = x.ToLatitude,
                          Longitude = x.ToLongtitude
                      },
                      //Distance = x.Distance,
                      TripStatus = new TripResponseTripStatusDetailDto()
                      {
                          Id = x.TripStatus.Id,
                          Name = x.TripStatus.Name,
                          Color = x.TripStatus.Color,
                      },
                      TripRequest = x.TripRequest != null ? new TripResponseTripRequestDetailDto()
                      {
                          Id = x.TripRequest.Id,
                          TripRequestStatusId = x.TripRequest.TripRequestStatus.Id,
                          RequesterName = x.TripRequest.Requester.FirstName + " " + x.TripRequest.Requester.LastName,
                          RequesterPhone = x.TripRequest.Requester.PhoneNumber,
                          //RequestedAt = x.TripRequest.RequestedAt
                      } : null,
                      Vehicle = new TripResponseVehicleDetailDto()
                      {
                          Id = x.Vehicle.Id,
                          //LicensePlate = x.Vehicle.LicensePlate,
                          VehicleModelName = x.Vehicle.VehicleModel.Name,
                          VehicleBrandName = x.Vehicle.VehicleBranch.Name
                      },
                      Driver = new TripResponseDriverDetailDto()
                      {
                          Id = x.Driver.Id,
                          DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                          DriverPhone = x.Driver.Users.FirstOrDefault().PhoneNumber
                      },
                      //RejectedDetail = string.IsNullOrEmpty(x.RejectReason) ? null : new RejectedDetail()
                      //{
                      //    RejectReason = x.RejectReason,
                      //    RejectionTime = x.RejectionTime,
                      //    RejectedByUserId = x.RejectedByUserId,
                      //},
                      //CancelledDetail = string.IsNullOrEmpty(x.CancelReason) ? null : new CancelledDetail()
                      //{
                      //    CancelReason = x.CancelReason,
                      //    CancellationTime = x.CancellationTime,
                      //    CancelledByUserId = x.CancelledByUserId,
                      //},
                      Expenses = x.TripExpenses
                          .Where(x => !x.IsDeleted)
                          .Select(te => new TripResponseTripExpenseDetailDto
                          {
                              Id = te.Id,
                              ExpenseType = new DataItem<int>
                              {
                                  Id = te.ExpenseTypeId,
                                  Name = te.ExpenseType.Name
                              },
                              Amount = te.Amount,
                              Status = new DetailStatusDto<int>
                              {
                                  Id = te.Status
                              },
                              //ExpenseDate = te.ExpenseDate
                          })
                          .ToList(),
                      ApprovalBy = x.UpdatedBy,
                      //ApprovalByName = x.ApprovalUser.FirstName + " " + x.ApprovalUser.LastName,
                      //DriverSalary = x.DriverSalary,
                      //Revenue = x.Revenue,
                      CreatedBy = x.CreatedBy ?? 0,
                      CreatedDate = x.CreatedDate,
                  })
                  .FirstOrDefaultAsync();

            if (data == null)
                return ApiResponse.NotFound();

            if (data.Expenses.Any())
            {
                foreach (var expense in data.Expenses)
                {
                    //Load thêm Status
                    var status = CommonConstants.ApprovalStatuses
                        .FirstOrDefault(x => x.Id == expense.Status.Id);
                    expense.Status.Name = status?.Name ?? string.Empty;
                    expense.Status.Color = status?.Color ?? string.Empty;
                }

                var tripExpenseIds = data.Expenses
                    .Select(x => x.Id)
                    .ToList();

                //data.Attachments = await _tripExpenseAttachmentRepository
                //    .FindByCondition(x => tripExpenseIds.Contains(x.TripExpenseId))
                //    .Select(x => new FileUploadDetailDto
                //    {
                //        Id = x.FileId,
                //        FileName = x.FileUpload.FileName,
                //        FileSize = x.FileUpload.FileSize,
                //        FileKey = x.FileUpload.FileKey,
                //        FileType = x.FileUpload.FileType
                //    })
                //    .ToListAsync();

                //foreach (var attachment in data.Attachments)
                //{
                //    attachment.Url = _storageService.GetTemporaryUrl(attachment.FileKey);
                //}

                data.TotalExpense = data.Expenses.Sum(x => x.Amount);
            }
            return ApiResponse.Success(data);
        }

        //List trip server-side pagination
        public async Task<ApiResponse> GetPagedAsync(TripDTParameters parameters)
        {
            var data = await _tripRepository.GetPagedAsync(parameters);
            return ApiResponse.Success(data);
        }

        //Paged
        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _tripRepository
                  .FindByCondition(x => !x.IsDeleted)
                  .Select(x => new TripListDto()
                  {
                      Id = x.Id,
                      //Purpose = x.Purpose,
                      //Notes = x.Notes,
                      //ScheduledStartTime = x.ScheduledStartTime,
                      //ScheduledEndTime = x.ScheduledEndTime,
                      ActualStartTime = x.ActualStartTime,
                      ActualEndTime = x.ActualEndTime,
                      StartOdometer = x.StartOdoMeter,
                      EndOdometer = x.EndOdoMeter,
                      //DispatchTime = x.DispatchTime,
                      //ConfirmationTime = x.ConfirmationTime,
                      PickUpTime = x.PickUpTime,
                      //CancellationTime = x.CancellationTime,
                      FromLocation = x.FromLocation,
                      FromLatitude = x.FromLatitude,
                      FromLongitude = x.FromLongtitude,
                      ToLocation = x.ToLocation,
                      ToLatitude = x.ToLatitude,
                      ToLongitude = x.ToLongtitude,
                      TripStatusId = x.TripStatusId,
                      TripStatusName = x.TripStatus.Name,
                      TripStatusColor = x.TripStatus.Color,
                      VehicleId = x.VehicleId,
                      //VehicleLicensePlate = x.Vehicle.LicensePlate,
                      VehicleModelName = x.Vehicle.VehicleModel.Name,
                      DriverId = x.DriverId,
                      DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                      DriverPhone = x.Driver.Users.FirstOrDefault().PhoneNumber,
                      TripRequestId = x.TripRequestId,
                      RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : null,
                      RequesterName = x.TripRequest != null ? x.TripRequest.Requester.FirstName + " " + x.TripRequest.Requester.LastName : null,
                      RequesterPhone = x.TripRequest != null ? x.TripRequest.Requester.PhoneNumber : null,
                      //CancelReason = x.CancelReason,
                      CancelledByUserId = x.UpdatedBy,
                      ApprovalBy = x.UpdatedBy,
                      //ApprovalByName = x.ApprovalUser.FirstName + " " + x.ApprovalUser.LastName,
                      CreatedBy = (int)x.CreatedBy,
                      CreatedDate = x.CreatedDate,
                  });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data.Where(
                 x =>
                 x.TripStatusName.Contains(query.Keyword.ToLower()) ||
                 x.FromLocation.ToLower().Contains(query.Keyword.ToLower()) ||
                 x.ToLocation.ToLower().Contains(query.Keyword.ToLower()) ||
                 (x.RequesterName != null && x.RequesterName.ToLower().Contains(query.Keyword.ToLower())) ||
                 (x.RequesterPhone != null && x.RequesterPhone.ToLower().Contains(query.Keyword.ToLower())) ||
                 //x.VehicleLicensePlate.ToLower().Contains(query.Keyword.ToLower()) ||
                 x.DriverName.ToLower().Contains(query.Keyword.ToLower()) ||
                 (x.DriverPhone != null && x.DriverPhone.ToLower().Contains(query.Keyword.ToLower()))
                 //x.ApprovalByName.ToLower().Contains(query.Keyword.ToLower())
                );
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripListDto>
            {
                CurrentPage = query.PageIndex,
                PageSize = query.PageSize,
                DataSource = await data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await data.CountAsync()
            };
            return ApiResponse.Success(pagedData);
        }

        //Admin cancel trip
        public async Task<ApiResponse> CancelAsync(CancelTripDto obj)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();

            if (obj.CancelStatusId != CommonConstants.TripStatus.CANCELLED_BY_ADMIN &&
                obj.CancelStatusId != CommonConstants.TripStatus.CANCELLED_BY_USER &&
                obj.CancelStatusId != CommonConstants.TripStatus.CANCELLED_BY_DRIVER)
                return ApiResponse.UnprocessableEntity(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData).Replace("{PropertyName}", "Huỷ chuyến bởi"),
                    ApiCodeConstants.Common.InvalidData);

            await _tripRepository.BeginTransactionAsync();
            try
            {
                var listStatusNotAllow = new List<int>
                {
                    CommonConstants.TripStatus.CANCELLED_BY_ADMIN,
                    CommonConstants.TripStatus.CANCELLED_BY_USER,
                    CommonConstants.TripStatus.CANCELLED_BY_DRIVER,
                    CommonConstants.TripStatus.COMPLETED
                };

                // Lấy dữ liệu chuyến đi
                var data = await _tripRepository
                    .FindByCondition(x =>
                        x.Id == obj.TripId &&
                        !listStatusNotAllow.Contains(x.TripStatusId))
                    .Select(x => new
                    {
                        Trip = x,
                        TripRequest = x.TripRequest,
                        DriverUserId = x.Driver.Users.FirstOrDefault().Id,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_USER && data.TripRequest == null)
                    return ApiResponse.UnprocessableEntity(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData).Replace("{PropertyName}", "Huỷ chuyến bởi"),
                    ApiCodeConstants.Common.InvalidData);

                int? cancelledByUserId;
                if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_USER && data.TripRequest != null)
                    cancelledByUserId = data.TripRequest.RequesterId;
                else if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_DRIVER)
                    cancelledByUserId = data.DriverUserId;
                else
                    cancelledByUserId = currentUserId;

                // Cập nhật trạng thái trip
                var existTripData = data.Trip;
                existTripData.TripStatusId = obj.CancelStatusId;
                //existTripData.CancelledByUserId = cancelledByUserId;
                //existTripData.CancelReason = obj.CancelReason;
                //existTripData.CancellationTime = DateTime.Now;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData);

                // Cập nhật trạng thái trip request
                if (data.TripRequest != null)
                {
                    var existTripRequestData = data.TripRequest;
                    existTripRequestData.TripRequestStatusId = CommonConstants.TripRequestStatus.CANCELLED;
                    existTripRequestData.LastModifiedDate = DateTime.Now;
                    existTripRequestData.UpdatedBy = currentUserId;
                    //existTripData.CancelledByUserId = cancelledByUserId;
                    //existTripData.CancelReason = obj.CancelReason;

                    await _tripRequestRepository.UpdateAsync(existTripRequestData);
                }

                // Cập nhật vehicle
                var vehicle = await _vehicleRepository.GetByIdAsync(existTripData.VehicleId);
                if (vehicle == null)
                    return ApiResponse.BadRequest();

                vehicle.VehicleStatusId = CommonConstants.VehicleStatus.AVAILABLE;
                vehicle.UpdatedBy = currentUserId;
                vehicle.LastModifiedDate = DateTime.Now;
                _vehicleRepository.Update(vehicle);

                // Cập nhật driver
                var driver = await _driverRepository.GetByIdAsync(existTripData.DriverId);
                if (driver == null)
                    return ApiResponse.BadRequest();

                driver.DriverStatusId = CommonConstants.DriverStatus.AVAILABLE;
                driver.UpdatedBy = currentUserId;
                driver.LastModifiedDate = DateTime.Now;
                await _driverRepository.UpdateAsync(driver);


                //#region Gửi thông báo
                //var listUserForNoti = new List<int>();
                //string notiTitle = string.Empty;
                //string notiContent = string.Empty;

                //if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_ADMIN)
                //{
                //    notiTitle = NotificationConstants.TripMessages.CancelledByDispatcher_Title;
                //    notiContent = string.Format(NotificationConstants.TripMessages.CancelledByDispatcher_Body, data.Trip.FromLocation, data.Trip.ToLocation);
                //    listUserForNoti.Add(driver.UserId);
                //    if (data.TripRequest != null)
                //        listUserForNoti.Add(data.TripRequest.RequesterId);
                //}
                //else if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_DRIVER)
                //{
                //    notiTitle = NotificationConstants.TripMessages.CancelledByDriver_Title;
                //    notiContent = string.Format(NotificationConstants.TripMessages.CancelledByDriver_Body_Dispatcher, data.DriverName);
                //    listUserForNoti.Add(currentUserId ?? 0);
                //    if (data.TripRequest != null)
                //        listUserForNoti.Add(data.TripRequest.RequesterId);
                //}
                //else if (obj.CancelStatusId == CommonConstants.TripStatus.CANCELLED_BY_USER && data.TripRequest != null)
                //{
                //    var requester = await _userRepository
                //        .FindByCondition(x => x.Id == data.TripRequest.RequesterId)
                //        .Select(x => new
                //        {
                //            x.Id,
                //            FullName = x.FirstName + " " + x.LastName
                //        })
                //        .FirstOrDefaultAsync();
                //    if (requester == null)
                //        return ApiResponse.BadRequest();

                //    notiTitle = NotificationConstants.TripMessages.CancelledByUser_Title;
                //    notiContent = string.Format(NotificationConstants.TripMessages.CancelledByUser_Body, requester.FullName, data.Trip.FromLocation, data.Trip.ToLocation);
                //    listUserForNoti.Add(data.TripRequest.RequesterId);
                //    listUserForNoti.Add(data.Trip.ApprovalBy);
                //}

                //var notifications = listUserForNoti
                //    .Select(x => new Notification
                //    {
                //        Title = notiTitle,
                //        Content = notiContent,
                //        DirectionId = data.Trip.Id.ToString(),
                //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER,
                //        UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = x,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy = CommonConstants.ADMIN_USER
                //            }
                //        }
                //    })
                //    .ToList();

                //await _notificationRepository.CreateListAsync(notifications);
                //#endregion

                await _tripRepository.SaveChangesAsync();
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel trip with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }


        //Driver update status accept trip
        public async Task<ApiResponse> DriverAcceptTrip(DriverUpdateTripDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            await _tripRepository.BeginTransactionAsync();
            try
            {
                var data = await _tripRepository.FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.DISPATCHED)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;

                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                // Cập nhật trạng thái chuyến đi
                existTripData.TripStatusId = CommonConstants.TripStatus.DRIVER_CONFIRMED;
                //existTripData.ConfirmationTime = DateTime.Now;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData);

                //#region Gửi thông báo
                //var notifications = new List<Notification>();
                //string tripIdStr = existTripData.Id.ToString();

                //// Điều phối viên
                //string contentDispatcher = string.Format(
                //    NotificationConstants.TripMessages.DriverAccepted_Body_Dispatcher,
                //    data.DriverName,
                //    existTripData.FromLocation,
                //    existTripData.ToLocation
                //);

                //notifications.Add(new Notification
                //{
                //    Title = NotificationConstants.TripMessages.DriverAccepted_Title,
                //    Content = contentDispatcher,
                //    DirectionId = tripIdStr,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            UserId = existTripData.ApprovalBy,
                //            IsRead = false,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        }
                //    }
                //});

                //// Người dùng
                //if (data.RequesterId != 0)
                //{
                //    string contentRequester = string.Format(
                //        NotificationConstants.TripMessages.DriverAccepted_Body,
                //        data.DriverName,
                //        existTripData.FromLocation,
                //        existTripData.ScheduledStartTime.ToString("dd/MM/yyyy HH:mm")
                //    );

                //    notifications.Add(new Notification
                //    {
                //        Title = NotificationConstants.TripMessages.DriverAccepted_Title,
                //        Content = contentRequester,
                //        DirectionId = tripIdStr,
                //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER,
                //        UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = data.RequesterId,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy = CommonConstants.ADMIN_USER
                //            }
                //        }
                //    });
                //}
                //await _notificationRepository.CreateListAsync(notifications);

                //#endregion
                await _tripRepository.SaveChangesAsync(); // Lưu cập nhật
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status driver confirmed by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update status reject trip
        public async Task<ApiResponse> DriverRejectTrip(DriverRejectTripDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            await _tripRepository.BeginTransactionAsync();
            try
            {
                var data = await _tripRepository.FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.DISPATCHED)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                        Driver = x.Driver,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;

                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                // Cập nhật trạng thái chuyến đi
                existTripData.TripStatusId = CommonConstants.TripStatus.REJECTED_BY_DRIVER;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                //existTripData.RejectReason = obj.RejectReason;
                //existTripData.RejectionTime = DateTime.Now;
                //existTripData.RejectedByUserId = currentUserId;
                await _tripRepository.UpdateAsync(existTripData);

                // Cập nhật trạng thái tài xế
                var driverData = data.Driver;
                driverData.DriverStatusId = CommonConstants.DriverStatus.AVAILABLE;
                driverData.UpdatedBy = currentUserId;
                driverData.LastModifiedDate = DateTime.Now;
                await _driverRepository.UpdateAsync(driverData);

                //#region Gửi thông báo
                //var notifications = new List<Notification>();
                //string tripIdStr = existTripData.Id.ToString();

                //// Điều phối viên
                //string contentDispatcher = string.Format(
                //    NotificationConstants.TripMessages.DriverRejected_Body,
                //    data.DriverName
                //);

                //notifications.Add(new Notification
                //{
                //    Title = NotificationConstants.TripMessages.DriverRejected_Title,
                //    Content = contentDispatcher,
                //    DirectionId = tripIdStr,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            UserId = existTripData.ApprovalBy,
                //            IsRead = false,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        }
                //    }
                //});

                //await _notificationRepository.CreateListAsync(notifications);

                //#endregion
                await _tripRepository.SaveChangesAsync(); // Lưu cập nhật
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status driver confirmed by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update status moving to pickup
        public async Task<ApiResponse> DriverMovingToPickup(DriverUpdateTripDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            await _tripRepository.BeginTransactionAsync();
            try
            {
                var data = await _tripRepository.FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.DRIVER_CONFIRMED)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;

                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                // Cập nhật trạng thái
                existTripData.TripStatusId = CommonConstants.TripStatus.MOVING_TO_PICKUP;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData);

                //#region Gửi thông báo
                //var notifications = new List<Notification>();
                //string driverName = data.DriverName;
                //string tripIdStr = existTripData.Id.ToString();

                //// Điều phối viên
                //string contentDispatcher = string.Format(
                //    NotificationConstants.TripMessages.DriverMovingToPickup_Body_Dispatcher,
                //    driverName,
                //    existTripData.FromLocation);

                //notifications.Add(new Notification
                //{
                //    Title = NotificationConstants.TripMessages.DriverMovingToPickup_Title,
                //    Content = contentDispatcher,
                //    DirectionId = tripIdStr,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            UserId = existTripData.ApprovalBy,
                //            IsRead = false,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER

                //        }
                //    }
                //});

                //// Người yêu cầu (requester)
                //if (data.RequesterId != 0)
                //{
                //    string contentRequester = string.Format(
                //        NotificationConstants.TripMessages.DriverMovingToPickup_Body,
                //        driverName,
                //        existTripData.FromLocation);

                //    notifications.Add(new Notification
                //    {
                //        Title = NotificationConstants.TripMessages.DriverMovingToPickup_Title,
                //        Content = contentRequester,
                //        DirectionId = tripIdStr,
                //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER,
                //        UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = data.RequesterId,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy = CommonConstants.ADMIN_USER
                //            }
                //        }
                //    });
                //}
                //await _notificationRepository.CreateListAsync(notifications);
                //#endregion

                await _tripRepository.SaveChangesAsync();
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status moving to pickup by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update status arrived at pickup
        public async Task<ApiResponse> DriverArrivedAtPickup(DriverUpdateTripDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            await _tripRepository.BeginTransactionAsync();
            try
            {
                var data = await _tripRepository.FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.MOVING_TO_PICKUP)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;

                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                // Cập nhật trạng thái trip
                existTripData.TripStatusId = CommonConstants.TripStatus.ARRIVED_AT_PICKUP;
                existTripData.PickUpTime = DateTime.Now;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData); // Lưu cập nhật trạng thái

                //#region Gửi thông báo requester
                //var notifications = new List<Notification>();

                //if (data.RequesterId != 0)
                //{
                //    string driverName = data.DriverName;
                //    string tripIdStr = existTripData.Id.ToString();

                //    string content = string.Format(
                //        NotificationConstants.TripMessages.ArrivedAtPickupPoint_Body,
                //        driverName,
                //        existTripData.FromLocation);

                //    notifications.Add(new Notification
                //    {
                //        Title = NotificationConstants.TripMessages.ArrivedAtPickupPoint_Title,
                //        Content = content,
                //        DirectionId = tripIdStr,
                //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER,
                //        UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = data.RequesterId,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy= CommonConstants.ADMIN_USER,
                //            }
                //        }
                //    });
                //}
                //await _notificationRepository.CreateListAsync(notifications);
                //#endregion

                await _tripRepository.SaveChangesAsync(); // Lưu cập nhật trạng thái
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status arrived at pickup by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update status moving to destination
        public async Task<ApiResponse> DriverMovingToDestination(DriverUpdateTripMovingToDestinationDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            await _tripRepository.BeginTransactionAsync();
            try
            {
                var data = await _tripRepository.FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.ARRIVED_AT_PICKUP)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;

                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                var historyTripIds = await _tripRepository
                    .FindByCondition(x => x.TripStatusId == CommonConstants.TripStatus.COMPLETED
                        && x.VehicleId == existTripData.VehicleId && x.EndOdoMeter != null)
                    .Select(x => x.Id)
                    .ToListAsync();

                if (historyTripIds != null && historyTripIds.Any())
                {
                    var lastTripId = historyTripIds.Max();
                    var lastEndOdometer = await _tripRepository
                        .FindByCondition(x => x.Id == lastTripId)
                        .Select(x => x.EndOdoMeter)
                        .FirstOrDefaultAsync();
                    if (lastEndOdometer != null && obj.StartOdometer < lastEndOdometer)
                    {
                        return ApiResponse.UnprocessableEntity(
                            ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanOrEqualMessage)
                            .Replace("{PropertyName}", "Số Km trên công tơ mét")
                            .Replace("{ComparisonProperty}", "Số Km trên công tơ mét của chuyến gần nhất"),
                            ApiCodeConstants.Common.GreaterThanOrEqualMessage);
                    }
                }

                // Cập nhật thông tin chuyến đi
                existTripData.ActualStartTime = DateTime.Now;
                existTripData.TripStatusId = CommonConstants.TripStatus.MOVING_TO_DESTINATION;
                existTripData.StartOdoMeter = obj.StartOdometer;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData);

                //#region Gửi thông báo điều phối viên và người dùng
                //string driverName = data.DriverName;
                //string tripIdStr = existTripData.Id.ToString();

                //string content = string.Format(
                //    NotificationConstants.TripMessages.Started_Body,
                //    driverName,
                //    existTripData.FromLocation,
                //    existTripData.ToLocation);

                //var notification = new Notification
                //{
                //    Title = NotificationConstants.TripMessages.Started_Title,
                //    Content = content,
                //    DirectionId = tripIdStr,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            UserId = existTripData.ApprovalBy,
                //            IsRead = false,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER
                //        }
                //    }
                //};
                //if (data.RequesterId > 0)
                //{
                //    notification.UserNotifications.Add(new UserNotification
                //    {
                //        UserId = data.RequesterId,
                //        IsRead = false,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER
                //    });
                //}
                //await _notificationRepository.CreateAsync(notification);
                //#endregion

                await _tripRepository.SaveChangesAsync();
                await _tripRepository.EndTransactionAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(new List<Notification> { notification });

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status moving to destination by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update status arrived at destination
        public async Task<ApiResponse> DriverArrivedAtDestination(DriverUpdateTripDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            try
            {
                var data = await _tripRepository
                    .FindByCondition(x => x.Id == obj.TripId && x.TripStatusId == CommonConstants.TripStatus.MOVING_TO_DESTINATION)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterName = x.TripRequest != null ? x.TripRequest.Requester.FirstName + " " + x.TripRequest.Requester.LastName : string.Empty,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();
                var trip = data.Trip;
                if (trip.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                // Cập nhật trạng thái chuyến đi (đã được tracking nên không cần gọi UpdateAsync)
                trip.TripStatusId = CommonConstants.TripStatus.ARRIVED_AT_DESTINATION;
                trip.ActualEndTime = DateTime.Now;
                trip.UpdatedBy = currentUserId;
                trip.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(trip);

                //#region Gửi thông báo điều phối viên và người dùng
                //var notifications = new List<Notification>();

                //if (data.RequesterId > 0)
                //{
                //    var notificationForUser = new Notification
                //    {
                //        Title = NotificationConstants.TripMessages.ArrivedAtDropoffPoint_Title_User,
                //        Content = NotificationConstants.TripMessages.ArrivedAtDropoffPoint_Body_User,
                //        DirectionId = trip.Id.ToString(),
                //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //        CreatedDate = DateTime.Now,
                //        CreatedBy = CommonConstants.ADMIN_USER,
                //        UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = data.RequesterId,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy = CommonConstants.ADMIN_USER
                //            }
                //        }
                //    };
                //    notifications.Add(notificationForUser);
                //}

                //var notificationForDistpatcher = new Notification
                //{
                //    Title = NotificationConstants.TripMessages.ArrivedAtDropoffPoint_Title_Dispatcher,
                //    Content = string.Format(NotificationConstants.TripMessages.ArrivedAtDropoffPoint_Body_Dispatcher,
                //        data.DriverName, data.RequesterName, trip.ToLocation, DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
                //    DirectionId = trip.Id.ToString(),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //        {
                //            new UserNotification
                //            {
                //                UserId = trip.ApprovalBy,
                //                IsRead = false,
                //                CreatedDate = DateTime.Now,
                //                CreatedBy = CommonConstants.ADMIN_USER
                //            }
                //        }
                //};
                //notifications.Add(notificationForDistpatcher);

                //await _notificationRepository.CreateListAsync(notifications);

                //#endregion

                await _tripRepository.SaveChangesAsync();

                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status arrived at destination by driver with message {Message}", ex.Message);
                return ApiResponse.InternalServerError();
            }
        }

        //Driver update complete trip
        public async Task<ApiResponse> DriverCompleteTrip(DriverUpdateTripCompleteDto obj)
        {
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
            try
            {
                await _tripRepository.BeginTransactionAsync();

                var data = await _tripRepository
                    .FindByCondition(x =>
                        x.Id == obj.TripId &&
                        x.TripStatusId == CommonConstants.TripStatus.ARRIVED_AT_DESTINATION)
                    .Select(x => new
                    {
                        Trip = x,
                        DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                        RequesterId = x.TripRequest != null ? x.TripRequest.RequesterId : 0,
                    })
                    .FirstOrDefaultAsync();

                if (data == null) return ApiResponse.NotFound();

                var existTripData = data.Trip;
                if (existTripData.DriverId != currentDriverId)
                    return ApiResponse.Forbidden(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                        ApiCodeConstants.Common.Forbidden);

                if (existTripData.StartOdoMeter > obj.EndOdometer)
                {
                    return ApiResponse.UnprocessableEntity(
                        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanMessage)
                        .Replace("{PropertyName}", "Số Km trên công tơ mét khi hoàn thành chuyến")
                        .Replace("{ComparisonProperty}", "Số Km trên công tơ mét khi bắt đầu chuyến"),
                        ApiCodeConstants.Common.GreaterThanMessage);
                }

                // Cập nhật thông tin chuyến đi
                existTripData.TripStatusId = CommonConstants.TripStatus.COMPLETED;
                existTripData.EndOdoMeter = obj.EndOdometer;
                //existTripData.CompletionTime = DateTime.Now;
                existTripData.UpdatedBy = currentUserId;
                existTripData.LastModifiedDate = DateTime.Now;
                await _tripRepository.UpdateAsync(existTripData);

                // Cập nhật trạng thái xe
                var vehicle = await _vehicleRepository.GetByIdAsync(existTripData.VehicleId);
                if (vehicle != null)
                {
                    vehicle.VehicleStatusId = CommonConstants.VehicleStatus.AVAILABLE;
                    vehicle.UpdatedBy = currentUserId;
                    vehicle.LastModifiedDate = DateTime.Now;
                    _vehicleRepository.Update(vehicle);
                }

                // Cập nhật trạng thái tài xế
                var driver = await _driverRepository.GetByIdAsync(existTripData.DriverId);
                if (driver != null)
                {
                    driver.DriverStatusId = CommonConstants.DriverStatus.AVAILABLE;
                    driver.UpdatedBy = currentUserId;
                    driver.LastModifiedDate = DateTime.Now;
                    await _driverRepository.UpdateAsync(driver);
                }

                //#region Gửi thông báo
                //string driverName = data.DriverName;
                //string tripIdStr = existTripData.Id.ToString();
                ////string nowStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                //var notifications = new List<Notification>();

                //// Thông báo điều phối viên
                //var dispatcherNotification = new Notification
                //{
                //    Title = NotificationConstants.TripMessages.Completed_Title,
                //    Content = string.Format(
                //        NotificationConstants.TripMessages.Completed_Body_Dispatcher,
                //        driverName,
                //        existTripData.FromLocation,
                //        existTripData.ToLocation),
                //    DirectionId = tripIdStr,
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                //    CreatedDate = DateTime.Now,
                //    CreatedBy = CommonConstants.ADMIN_USER,
                //    UserNotifications = new List<UserNotification>
                //    {
                //        new UserNotification
                //        {
                //            UserId = existTripData.ApprovalBy,
                //            IsRead = false,
                //            CreatedDate = DateTime.Now,
                //            CreatedBy = CommonConstants.ADMIN_USER

                //        }
                //    }
                //};
                //notifications.Add(dispatcherNotification);

                //// Thông báo người dùng
                ////if (data.RequesterId != 0)
                ////{
                ////    var userNotification = new Notification
                ////    {
                ////        Title = NotificationConstants.TripMessages.Completed_Title,
                ////        Content = string.Format(
                ////            NotificationConstants.TripMessages.Completed_Body_User,
                ////            nowStr),
                ////        DirectionId = tripIdStr,
                ////        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP,
                ////        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                ////        CreatedDate = DateTime.Now,
                ////        CreatedBy = CommonConstants.ADMIN_USER,

                ////        UserNotifications = new List<UserNotification>
                ////        {
                ////            new UserNotification
                ////            {
                ////                UserId = data.RequesterId,
                ////                IsRead = false,
                ////                CreatedDate = DateTime.Now,
                ////                CreatedBy = CommonConstants.ADMIN_USER

                ////            }
                ////        }
                ////    };
                ////    notifications.Add(userNotification);
                ////}

                //await _notificationRepository.CreateListAsync(notifications);
                //#endregion
                await _tripRepository.SaveChangesAsync(); // Save cho Trip, Vehicle, Driver
                await _tripRepository.EndTransactionAsync();
                //Gửi FCM
                //await SendNotificationsWithFcmAsync(notifications);
                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete trip by driver with message {Message}", ex.Message);
                await _tripRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }

        //public Task<ApiResponse> GetPagedAsync<T>(AdvancedSearchQuery<T> query)
        //{
        //    throw new NotImplementedException();
        //}
        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateTripDto> obj)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse> CreateListAsync(IEnumerable<CreateTripDto> objs)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse> GetAllAsync()
        {
            // Triển khai logic lấy tất cả các chuyến đi
            var data = await _tripRepository
                  .FindByCondition(x => !x.IsDeleted)
                  // Sử dụng phép chiếu trung gian để lấy các đối tượng liên quan
                  .Select(x => new
                  {
                      Trip = x,
                      DriverUser = x.Driver.Users.FirstOrDefault(),
                      RequesterUser = x.TripRequest != null ? x.TripRequest.Requester : null,
                      TripStatus = x.TripStatus,
                      VehicleModel = x.Vehicle.VehicleModel
                  })
                  .Select(x => new TripListDto()
                  {
                      Id = x.Trip.Id,
                      ActualStartTime = x.Trip.ActualStartTime,
                      ActualEndTime = x.Trip.ActualEndTime,
                      StartOdometer = x.Trip.StartOdoMeter,
                      EndOdometer = x.Trip.EndOdoMeter,
                      PickUpTime = x.Trip.PickUpTime,
                      FromLocation = x.Trip.FromLocation,
                      FromLatitude = x.Trip.FromLatitude,
                      FromLongitude = x.Trip.FromLongtitude,
                      ToLocation = x.Trip.ToLocation,
                      ToLatitude = x.Trip.ToLatitude,
                      ToLongitude = x.Trip.ToLongtitude,
                      TripStatusId = x.Trip.TripStatusId,

                      // Kiểm tra null an toàn
                      TripStatusName = x.TripStatus != null ? x.TripStatus.Name : null,
                      TripStatusColor = x.TripStatus != null ? x.TripStatus.Color : null,

                      VehicleId = x.Trip.VehicleId,

                      VehicleModelName = x.VehicleModel != null ? x.VehicleModel.Name : null,

                      DriverId = x.Trip.DriverId,

                      DriverName = x.DriverUser != null ? x.DriverUser.FirstName + " " + x.DriverUser.LastName : null,
                      DriverPhone = x.DriverUser != null ? x.DriverUser.PhoneNumber : null,

                      TripRequestId = x.Trip.TripRequestId,

                      RequesterId = x.RequesterUser != null ? x.RequesterUser.Id : null,
                      RequesterName = x.RequesterUser != null ? x.RequesterUser.FirstName + " " + x.RequesterUser.LastName : null,
                      RequesterPhone = x.RequesterUser != null ? x.RequesterUser.PhoneNumber : null,

                      CancelledByUserId = x.Trip.UpdatedBy,
                      ApprovalBy = x.Trip.UpdatedBy,
                      CreatedBy = (int)(x.Trip.CreatedBy ?? 0), // Xử lý CreatedBy có thể null
                      CreatedDate = x.Trip.CreatedDate,
                  })
                  .ToListAsync(); // Thêm await và ToListAsync

            return ApiResponse.Success(data); // Trả về dữ liệu
        }

        public async Task<ApiResponse> GetTripByCurrentUser()
        {
            // Lấy thông tin người dùng hiện tại
            var currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var currentRoleIds = _httpContextAccessor.HttpContext?.GetCurrentRoleIds() ?? new List<int>();
            var currentDriverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId() ?? 0;

            // Kiểm tra vai trò
            bool isDispatcherOrAdmin = currentRoleIds.Any(roleId =>
                roleId == CommonConstants.Role.ADMIN ||
                roleId == CommonConstants.Role.DISPATCHER);

            bool isDriver = currentRoleIds.Contains(CommonConstants.Role.DRIVER);

            var query = _tripRepository.FindByCondition(x => !x.IsDeleted);

            // 1. Nếu là Admin/ĐPV, không lọc gì cả
            if (isDispatcherOrAdmin)
            {
                // Giữ nguyên query
            }
            // 2. Nếu là Lái xe, lọc theo DriverId
            else if (isDriver)
            {
                query = query.Where(x => x.DriverId == currentDriverId);
            }
            // 3. Nếu là Người dùng, lọc theo RequesterId
            else
            {
                query = query.Where(x => x.TripRequest.RequesterId == currentUserId);
            }

            // Triển khai logic lấy tất cả các chuyến đi
            var data = await query
                  .Select(x => new
                  {
                      Trip = x,
                      DriverUser = x.Driver.Users.FirstOrDefault(),
                      RequesterUser = x.TripRequest != null ? x.TripRequest.Requester : null,
                      TripStatus = x.TripStatus,
                      VehicleModel = x.Vehicle.VehicleModel
                  })
                  .Select(x => new TripListDto()
                  {
                      Id = x.Trip.Id,
                      ActualStartTime = x.Trip.ActualStartTime,
                      ActualEndTime = x.Trip.ActualEndTime,
                      StartOdometer = x.Trip.StartOdoMeter,
                      EndOdometer = x.Trip.EndOdoMeter,
                      PickUpTime = x.Trip.PickUpTime,
                      FromLocation = x.Trip.FromLocation,
                      FromLatitude = x.Trip.FromLatitude,
                      FromLongitude = x.Trip.FromLongtitude,
                      ToLocation = x.Trip.ToLocation,
                      ToLatitude = x.Trip.ToLatitude,
                      ToLongitude = x.Trip.ToLongtitude,
                      TripStatusId = x.Trip.TripStatusId,
                      TripStatusName = x.TripStatus != null ? x.TripStatus.Name : null,
                      TripStatusColor = x.TripStatus != null ? x.TripStatus.Color : null,
                      VehicleId = x.Trip.VehicleId,
                      VehicleModelName = x.VehicleModel != null ? x.VehicleModel.Name : null,
                      DriverId = x.Trip.DriverId,
                      DriverName = x.DriverUser != null ? x.DriverUser.FirstName + " " + x.DriverUser.LastName : null,
                      DriverPhone = x.DriverUser != null ? x.DriverUser.PhoneNumber : null,
                      TripRequestId = x.Trip.TripRequestId,
                      RequesterId = x.RequesterUser != null ? x.RequesterUser.Id : null,
                      RequesterName = x.RequesterUser != null ? x.RequesterUser.FirstName + " " + x.RequesterUser.LastName : null,
                      RequesterPhone = x.RequesterUser != null ? x.RequesterUser.PhoneNumber : null,
                      CancelledByUserId = x.Trip.UpdatedBy,
                      ApprovalBy = x.Trip.UpdatedBy,
                      CreatedBy = (int)(x.Trip.CreatedBy ?? 0),
                      CreatedDate = x.Trip.CreatedDate,
                  })
                  .ToListAsync();

            return ApiResponse.Success(data);
        }

        //private async Task SendNotificationsWithFcmAsync(List<Notification> notifications)
        //{
        //    try
        //    {
        //        if (notifications.Count > 0)
        //        {
        //            var userIds = notifications
        //            .SelectMany(n => n.UserNotifications.Select(u => u.UserId))
        //            .Distinct()
        //            .ToList();

        //            var tokensByUser = (await _userDeviceRepository
        //                .FindByCondition(x => userIds.Contains(x.UserId) && !x.IsDeleted && !string.IsNullOrEmpty(x.DeviceToken))
        //                .Select(x => new { x.UserId, x.DeviceToken })
        //                .ToListAsync())
        //                .GroupBy(x => x.UserId)
        //                .ToDictionary(g => g.Key, g => g.Select(t => t.DeviceToken).ToList());

        //            foreach (var noti in notifications)
        //            {
        //                var deviceTokens = noti.UserNotifications
        //                    .SelectMany(u => tokensByUser.TryGetValue(u.UserId, out var tokens) ? tokens : Enumerable.Empty<string>())
        //                    .Distinct()
        //                    .ToList();

        //                if (deviceTokens.Count > 0)
        //                {
        //                    _fireBaseService.SendNotificationAsync(
        //                        deviceTokens,
        //                        noti.Title,
        //                        noti.Content,
        //                        noti.NotificationCategoryId.ToString(),
        //                        noti.DirectionId
        //                    );
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "[FCM] Failed to send notification: {Message}", ex.Message);
        //    }

        //}
    }
}
