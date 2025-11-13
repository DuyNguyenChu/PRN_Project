using api.Dtos.TripExpense;
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
    public class TripExpenseService : ITripExpenseService
    {
        private readonly ITripExpenseRepository _tripExpenseRepository;
        //private readonly ITripExpenseAttachmentRepository _tripExpenseAttachmentRepository;
        private readonly ITripRepository _tripRepository;
        //private readonly IStorageService _storageService;
        private readonly ILogger<TripExpenseService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        //private readonly IUserDeviceRepository _userDeviceRepository;
        //private readonly IFireBaseService _fireBaseService;

        public TripExpenseService(ILogger<TripExpenseService> logger, ITripExpenseRepository tripExpenseRepository, ITripRepository tripRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _tripExpenseRepository = tripExpenseRepository;
            //_tripExpenseAttachmentRepository = tripExpenseAttachmentRepository;
            _tripRepository = tripRepository;
            //_storageService = storageService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //_fireBaseService = fireBaseService;
            //_notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            //_userDeviceRepository = userDeviceRepository;
        }

        #region Services cho lái xe
        public async Task<ApiResponse> CreateAsync(CreateTripExpenseDto obj)
        {
            obj.CreatedBy = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;

            var driver = await _userRepository.GetByIdAsync(obj.CreatedBy ?? 0);
            if (driver == null)
                return ApiResponse.BadRequest();

            var tripDetail = await _tripRepository
                .FindByCondition(x => x.Id == obj.TripId)
                .Select(x => new
                {
                    DriverId = x.DriverId,
                    DriverUserId = x.Driver.Users.FirstOrDefault().Id,
                    VehicleId = x.VehicleId
                })
                .FirstOrDefaultAsync();

            if (tripDetail == null) return ApiResponse.NotFound();

            if (tripDetail.DriverUserId != obj.CreatedBy)
            {
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);
            }
            //var listDistpatcher = await (from a in _userRepository.GetAll()
            //                             join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
            //                             where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
            //                             select a.Id)
            //                        .ToListAsync();

            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => listDistpatcher.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            await _tripExpenseRepository.BeginTransactionAsync();
            try
            {
                var model = obj.ToEntity();
                model.DriverId = tripDetail.DriverId;
                model.VehicleId = tripDetail.VehicleId;

                await _tripExpenseRepository.CreateAsync(model);
                await _tripExpenseRepository.SaveChangesAsync(); // Save để có Id sinh mã

                //if (obj.TripExpenseAttachments != null && obj.TripExpenseAttachments.Any())
                //{
                //    var attachments = obj.TripExpenseAttachments
                //        .Select(x => new TripExpenseAttachment
                //        {
                //            TripExpenseId = model.Id,
                //            FileId = x.FileId,
                //            CreatedBy = obj.CreatedBy,
                //            CreatedDate = DateTime.Now,
                //        })
                //        .ToList();

                //    await _tripExpenseAttachmentRepository.CreateListAsync(attachments);
                //    //await _tripExpenseAttachmentRepository.SaveChangesAsync();
                //}

                //Gửi thông báo              
                //var notification = new Notification
                //{
                //    CreatedDate = DateTime.Now,
                //    Title = TripExpenseMessages.Created_Title,
                //    Content = string.Format(TripExpenseMessages.Created_Body, driver.FirstName + " " + driver.LastName),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_EXPENSE,
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

                await _tripExpenseRepository.EndTransactionAsync();

                //push notification
                //_fireBaseService.SendNotificationAsync(listFcmToken,
                //    notification.Title,
                //    notification.Content,
                //    notification.NotificationCategoryId.ToString(),
                //    model.Id.ToString()
                //);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Trip Expense with message {Message}", ex.Message);
                await _tripExpenseRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }
        public async Task<ApiResponse> CreateListAsync(int tripId, IEnumerable<TripCreateTripExpenseDto> objs)
        {
            int currentUserId = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var tripDetail = await _tripRepository
                .FindByCondition(x => x.Id == tripId)
                .Select(x => new
                {
                    DriverId = x.DriverId,
                    DriverUserId = x.Driver.Users.FirstOrDefault().Id,
                    VehicleId = x.VehicleId
                })
                .FirstOrDefaultAsync();

            //Nếu chuyến không tồn tại
            if (tripDetail == null)
            {
                return ApiResponse.NotFound();
            }

            var driver = await _userRepository.GetByIdAsync(currentUserId);
            if (driver == null)
                return ApiResponse.BadRequest();

            //Nếu driver khác driver của chuyến thì không có quyền thêm chi phí
            if (currentUserId != tripDetail.DriverUserId)
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);

            // Lấy danh sách phân phối viên (dispatchers)
            //var listDistpatcher = await (from a in _userRepository.GetAll()
            //                             join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
            //                             where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
            //                             select a.Id)
            //                    .ToListAsync();
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => listDistpatcher.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            await _tripExpenseRepository.BeginTransactionAsync();
            try
            {
                //var notifications = new List<Notification>();
                foreach (var obj in objs)
                {
                    var model = obj.ToEntity(tripId, currentUserId);
                    model.DriverId = tripDetail.DriverId;
                    model.VehicleId = tripDetail.VehicleId;
                    //if (obj.TripExpenseAttachments != null && obj.TripExpenseAttachments.Any())
                    //{
                    //    model.TripExpenseAttachments = obj.TripExpenseAttachments
                    //        .Select(x => new TripExpenseAttachment
                    //        {
                    //            TripExpenseId = model.Id,
                    //            FileId = x.FileId,
                    //            CreatedBy = currentUserId,
                    //            CreatedDate = DateTime.Now,
                    //        })
                    //        .ToList();
                    //}
                    await _tripExpenseRepository.CreateAsync(model);
                    await _tripExpenseRepository.SaveChangesAsync(); // để có Id


                    //    var notification = new Notification
                    //    {
                    //        CreatedDate = DateTime.Now,
                    //        Title = TripExpenseMessages.Created_Title,
                    //        Content = string.Format(TripExpenseMessages.Created_Body, driver.FirstName + " " + driver.LastName),
                    //        NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_EXPENSE,
                    //        NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
                    //        DirectionId = model.Id.ToString(),
                    //        CreatedBy = CommonConstants.ADMIN_USER,
                    //        UserNotifications = listDistpatcher
                    //            .Select(x => new UserNotification
                    //            {
                    //                IsRead = false,
                    //                UserId = x,
                    //                CreatedDate = DateTime.Now,
                    //                CreatedBy = CommonConstants.ADMIN_USER
                    //            })
                    //            .ToList()
                    //    };

                    //    notifications.Add(notification);
                }

                //await _notificationRepository.CreateListAsync(notifications);
                //await _notificationRepository.SaveChangesAsync();

                await _tripExpenseRepository.EndTransactionAsync();

                // Push thông báo
                //foreach (var notification in notifications)
                //{
                //    _fireBaseService.SendNotificationAsync(listFcmToken,
                //        notification.Title,
                //        notification.Content,
                //        notification.NotificationCategoryId.ToString(),
                //        notification.DirectionId
                //    );
                //}

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create list of Trip Expenses: {Message}", ex.Message);

                await _tripExpenseRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }
        public async Task<ApiResponse> UpdateAsync(UpdateTripExpenseDto obj)
        {
            obj.UpdatedBy = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;

            var existData = await _tripExpenseRepository.GetByIdAsync(obj.Id);
            if (existData == null) return ApiResponse.NotFound();

            // Chỉ cho phép cập nhật nếu trạng thái là chờ duyệt
            if (existData.Status != (int)ApprovalStatus.Pending)
            {
                return ApiResponse.BadRequest();
            }

            if (existData.CreatedBy != obj.UpdatedBy)
            {
                return ApiResponse.Forbidden(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden),
                    ApiCodeConstants.Common.Forbidden
                );
            }

            var driver = await _userRepository.GetByIdAsync(obj.UpdatedBy ?? 0);
            if (driver == null)
                return ApiResponse.BadRequest();

            //var listDistpatcher = await (from a in _userRepository.GetAll()
            //                             join b in _userRoleRepository.GetAll() on a.Id equals b.UserId
            //                             where !a.IsDeleted && !b.IsDeleted && b.RoleId == CommonConstants.Role.DISPATCHER
            //                             select a.Id)
            //                            .ToListAsync();
            //var listFcmToken = await _userDeviceRepository
            //        .FindByCondition(x => listDistpatcher.Contains(x.UserId) && !string.IsNullOrEmpty(x.DeviceToken))
            //        .Select(x => x.DeviceToken)
            //        .ToListAsync();

            await _tripExpenseRepository.BeginTransactionAsync();
            try
            {
                obj.ToEntity(existData);
                await _tripExpenseRepository.UpdateAsync(existData);
                await _tripExpenseRepository.SaveChangesAsync();

                #region Cập nhật TripExpenseAttachment (Xóa hết + Thêm mới)

                // Xóa tất cả attachment cũ
                //await _tripExpenseAttachmentRepository
                //    .SoftDeleteAsync(x => x.TripExpenseId == obj.Id);

                //// Thêm mới toàn bộ attachment gửi lên (nếu có)
                //if (obj.TripExpenseAttachments != null && obj.TripExpenseAttachments.Any())
                //{
                //    var newAttachments = obj.TripExpenseAttachments
                //        .Select(x => new TripExpenseAttachment
                //        {
                //            TripExpenseId = obj.Id,
                //            FileId = x.FileId,
                //            CreatedBy = obj.UpdatedBy,
                //            CreatedDate = DateTime.Now,
                //        })
                //        .ToList();

                //    await _tripExpenseAttachmentRepository.CreateListAsync(newAttachments);
                //}

                #endregion

                //add noti
                //var notification = new Notification
                //{
                //    CreatedDate = DateTime.Now,
                //    Title = TripExpenseMessages.Updated_Title,
                //    Content = string.Format(TripExpenseMessages.Updated_Body, driver.FirstName + " " + driver.LastName),
                //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_EXPENSE,
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

                await _tripExpenseRepository.EndTransactionAsync();

                ////push notification             
                //_fireBaseService.SendNotificationAsync(listFcmToken,
                //    notification.Title,
                //    notification.Content,
                //    notification.NotificationCategoryId.ToString(),
                //    existData.Id.ToString()
                //);

                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Trip Expense with message {Message}", ex.Message);
                await _tripExpenseRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }
        #endregion
        #region Services cho phân phối viên
        //Duyệt chi phí
        public async Task<ApiResponse> ApproveAsync(ApproveTripExpenseDto obj)
        {
            obj.ApprovedBy = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var existData = await _tripExpenseRepository
                .FindByCondition(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending)
                .Select(x => new
                {
                    TripExpense = x,
                    TripApprovalBy = x.Trip.UpdatedBy
                })
                .FirstOrDefaultAsync();

            //Nếu không có data
            if (existData == null)
                return ApiResponse.BadRequest();

            //Phải là người quản lý chuyến
            if (existData.TripApprovalBy != obj.ApprovedBy)
            {
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);
            }

            //Cập nhật bản ghi
            var existDataTripExpense = existData.TripExpense;

            if (existDataTripExpense.CreatedBy == null)
                return ApiResponse.BadRequest();

            obj.ToEntity(existDataTripExpense);

            await _tripExpenseRepository.UpdateAsync(existDataTripExpense);
            await _tripExpenseRepository.SaveChangesAsync();

            //add motification
            //var notification = new Notification
            //{
            //    CreatedDate = DateTime.Now,
            //    Title = TripExpenseMessages.Approved_Title,
            //    Content = TripExpenseMessages.Approved_Body,
            //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_EXPENSE,
            //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
            //    DirectionId = existDataTripExpense.Id.ToString(),
            //    CreatedBy = CommonConstants.ADMIN_USER,
            //    UserNotifications = new List<UserNotification>
            //    {
            //        new UserNotification
            //        {
            //            IsRead = false,
            //            UserId = (int)existDataTripExpense.CreatedBy,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = CommonConstants.ADMIN_USER
            //        }
            //    }
            //};

            //await _notificationRepository.CreateAsync(notification);
            //await _notificationRepository.SaveChangesAsync();

            ////push notification
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => x.UserId == existDataTripExpense.CreatedBy && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            //_fireBaseService.SendNotificationAsync(listFcmToken,
            //    notification.Title,
            //    notification.Content,
            //    notification.NotificationCategoryId.ToString(),
            //    existDataTripExpense.Id.ToString()
            //);

            return ApiResponse.Success();
        }
        //Từ chối chi phí
        public async Task<ApiResponse> RejectAsync(RejectTripExpenseDto obj)
        {
            obj.RejectBy = _httpContextAccessor.HttpContext?.GetCurrentUserId() ?? 0;
            var existData = await _tripExpenseRepository
                .FindByCondition(x => x.Id == obj.Id && x.Status == (int)ApprovalStatus.Pending)
                .Select(x => new
                {
                    TripExpense = x,
                    TripApprovalBy = x.Trip.UpdatedBy
                }).FirstOrDefaultAsync();

            //Nếu không có data
            if (existData == null)
                return ApiResponse.BadRequest();

            //Phải là người quản lý chuyến
            if (existData.TripApprovalBy != obj.RejectBy)
            {
                return ApiResponse.Forbidden(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.Forbidden), ApiCodeConstants.Common.Forbidden);
            }

            //Cập nhật bản ghi
            var existDataTripExpense = existData.TripExpense;

            if (existDataTripExpense.CreatedBy == null)
                return ApiResponse.BadRequest();

            obj.ToEntity(existDataTripExpense);

            await _tripExpenseRepository.UpdateAsync(existDataTripExpense);
            await _tripExpenseRepository.SaveChangesAsync();

            ////add noti
            //var notification = new Notification
            //{
            //    CreatedDate = DateTime.Now,
            //    Title = TripExpenseMessages.Rejected_Title,
            //    Content = string.Format(TripExpenseMessages.Rejected_Body, existDataTripExpense.RejectReason),
            //    NotificationCategoryId = CommonConstants.NotificationCategory.TRIP_EXPENSE,
            //    NotificationTypeId = CommonConstants.NotificationType.SYSTEM,
            //    DirectionId = existDataTripExpense.Id.ToString(),
            //    CreatedBy = CommonConstants.ADMIN_USER,
            //    UserNotifications = new List<UserNotification>
            //    {
            //        new UserNotification
            //        {
            //            IsRead = false,
            //            UserId = (int)existDataTripExpense.CreatedBy,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = CommonConstants.ADMIN_USER
            //        }
            //    }
            //};

            //await _notificationRepository.CreateAsync(notification);
            //await _notificationRepository.SaveChangesAsync();

            ////push notification
            //var listFcmToken = await _userDeviceRepository
            //    .FindByCondition(x => x.UserId == existDataTripExpense.CreatedBy && !string.IsNullOrEmpty(x.DeviceToken))
            //    .Select(x => x.DeviceToken)
            //    .ToListAsync();

            //_fireBaseService.SendNotificationAsync(listFcmToken,
            //    notification.Title,
            //    notification.Content,
            //    notification.NotificationCategoryId.ToString(),
            //    existDataTripExpense.Id.ToString()
            //);

            return ApiResponse.Success();
        }
        #endregion
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _tripExpenseRepository
               .FindByCondition(x => x.Id == id)
               .Select(x => new TripExpenseDetailDto()
               {
                   Id = x.Id,
                   Vehicle = new TripExpenseResponseVehicleDetailDto()
                   {
                       Id = x.VehicleId,
                       LicensePlate = x.Vehicle.VehicleModel.Name,
                   },
                   Driver = new TripExpenseResponseDriverDetailDto()
                   {
                       Id = x.DriverId,
                       FullName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName
                   },
                   Trip = new TripExpenseResponseTripDetailDto()
                   {
                       Id = x.TripId,
                       //TripCode = x.Trip.TripCode
                   },
                   ExpenseType = new DataItem<int>
                   {
                       Id = x.ExpenseTypeId,
                       Name = x.ExpenseType.Name
                   },
                   Status = new DetailStatusDto<int>
                   {
                       Id = x.Status
                   },
                   Amount = x.Amount,
                   ExpenseDate = x.OccurenceDate,
                   Notes = x.Notes,
                   ApprovalBy = x.ApprovedBy,
                   //ApprovalByName = x.ApprovalUser != null ? x.ApprovalUser.FirstName + " " + x.ApprovalUser.LastName : null,
                   ApprovalDate = x.ApprovedDate,
                   RejectReason = x.RejectReason,
                   //TripExpenseAttachments = x.TripExpenseAttachments != null ? x.TripExpenseAttachments.Select(xx => new FileUploadDetailDto()
                   //{
                   //    Id = xx.FileId,
                   //    FileName = xx.FileUpload.FileName,
                   //    FileKey = xx.FileUpload.FileKey,
                   //    FileSize = xx.FileUpload.FileSize,
                   //    FileType = xx.FileUpload.FileType,
                   //}).ToList() : null,
                   CreatedBy = (int)x.CreatedBy,
                   CreatedDate = x.CreatedDate,
               }).FirstOrDefaultAsync();

            if (data == null) return ApiResponse.NotFound();
            //if (data.TripExpenseAttachments != null)
            //{
            //    foreach (var item in data.TripExpenseAttachments)
            //    {
            //        item.Url = _storageService.GetTemporaryUrl(item.FileKey);
            //    }
            //}

            var status = CommonConstants.ApprovalStatuses.FirstOrDefault(x => x.Id == data.Status.Id);
            data.Status.Color = status?.Color ?? string.Empty;
            data.Status.Name = status?.Name ?? string.Empty;
            return ApiResponse.Success(data);
        }
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _tripExpenseRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _tripExpenseRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }
        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _tripExpenseRepository
               .FindByCondition(x => !x.IsDeleted)
               .Select(x => new TripExpenseListDto()
               {
                   Id = x.Id,
                   VehicleId = x.VehicleId,
                   VehicleLicensePlate = x.Vehicle.VehicleModel.Name,
                   DriverId = x.DriverId,
                   DriverName = x.Driver.Users.FirstOrDefault().FirstName + " " + x.Driver.Users.FirstOrDefault().LastName,
                   TripId = x.TripId,
                   //TripCode = x.Trip.TripCode,
                   ExpenseTypeId = x.ExpenseTypeId,
                   ExpenseTypeName = x.ExpenseType.Name,
                   Amount = x.Amount,
                   ExpenseDate = x.OccurenceDate,
                   Notes = x.Notes,
                   Status = x.Status,
                   ApprovalBy = x.ApprovedBy,
                   ApprovalDate = x.ApprovedDate,
                   RejectReason = x.RejectReason,
                   CreatedBy = x.CreatedBy ?? 0,
                   CreatedDate = x.CreatedDate,
               });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data.Where(
                 x => 
                 x.VehicleLicensePlate.Contains(query.Keyword.ToLower()) ||
                 x.DriverName.Contains(query.Keyword.ToLower()) ||
                 x.ExpenseTypeName.Contains(query.Keyword.ToLower()) ||
                 x.Amount.ToString().Contains(query.Keyword.ToLower()) ||
                (x.Notes != null && x.Notes.ToLower().Contains(query.Keyword.ToLower())) ||
                (x.RejectReason != null && x.RejectReason.ToLower().Contains(query.Keyword.ToLower()))
                );
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripExpenseListDto>
            {
                CurrentPage = query.PageIndex,
                PageSize = query.PageSize,
                DataSource = await data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await data.CountAsync()
            };
            return ApiResponse.Success(pagedData);


        }
        public async Task<ApiResponse> GetPagedAsync(TripExpenseDTParameters parameters)
        {
            var data = await _tripExpenseRepository.GetPagedAsync(parameters);
            return ApiResponse.Success(data);
        }
        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateTripExpenseDto> objs)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse> GetStatus()
        {
            var data = CommonConstants.ApprovalStatuses
                .Select(x => new DetailStatusDto<int>
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color
                });

            return await Task.FromResult(ApiResponse.Success(data));
        }
        //public Task<ApiResponse> GetPagedAsync<T>(AdvancedSearchQuery<T> query)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateTripExpenseDto> objs)
        {
            await _tripExpenseRepository.BeginTransactionAsync();
            try
            {
                foreach (var obj in objs)
                {
                    var tripDetail = await _tripRepository.FindByCondition(x => x.Id == obj.TripId)
                        .Select(x => new
                        {
                            DriverId = x.DriverId,
                            VehicleId = x.VehicleId
                        }).FirstOrDefaultAsync();

                    if (tripDetail == null)
                    {
                        await _tripExpenseRepository.RollbackTransactionAsync();
                        return ApiResponse.NotFound();
                    }

                    var model = obj.ToEntity();
                    model.DriverId = tripDetail.DriverId;
                    model.VehicleId = tripDetail.VehicleId;

                    await _tripExpenseRepository.CreateAsync(model);
                    await _tripExpenseRepository.SaveChangesAsync(); // để có Id

                    //if (obj.TripExpenseAttachments != null && obj.TripExpenseAttachments.Any())
                    //{
                    //    var attachments = obj.TripExpenseAttachments.Select(x => new TripExpenseAttachment
                    //    {
                    //        TripExpenseId = model.Id,
                    //        FileId = x.FileId,
                    //        CreatedBy = obj.CreatedBy,
                    //        CreatedDate = DateTime.Now,
                    //    }).ToList();

                    //    await _tripExpenseAttachmentRepository.CreateListAsync(attachments);
                    //    await _tripExpenseAttachmentRepository.SaveChangesAsync();
                    //}

                    // Gửi thông báo (nếu cần cho từng cái)
                }

                await _tripExpenseRepository.EndTransactionAsync();
                return ApiResponse.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create list of Trip Expenses: {Message}", ex.Message);
                await _tripExpenseRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }
        }
        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResponse> GetAllAsync()
        {
            throw new NotImplementedException();
        }

    }
}
