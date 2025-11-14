using api.Dtos.Driver;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.Service
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IUserRepository _userRepository;
        //private readonly IStorageService _storageService;
        private readonly IFuelLogRepository _fuelLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IDriverSalaryRepository _driverSalaryRepository;
        private readonly IDriverViolationRepository _driverViolationRepository;
        private readonly IMaintenanceRecordRepository _maintenanceRecordRepository;
        private readonly ITripExpenseRepository _tripExpenseRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger<DriverService> _logger;

        public DriverService(IDriverRepository driverRepository, IUserRepository userRepository, IFuelLogRepository fuelLogRepository,
            IHttpContextAccessor httpContextAccessor, IDriverViolationRepository driverViolationRepository,
            IMaintenanceRecordRepository maintenanceRecordRepository, ITripExpenseRepository tripExpenseRepository, ILoggerFactory loggerFactory, IUserRoleRepository userRoleRepository, ITripRepository tripRepository)
        {
            _driverRepository = driverRepository;
            _userRepository = userRepository;
            //_storageService = storageService;
            _fuelLogRepository = fuelLogRepository;
            _httpContextAccessor = httpContextAccessor;
            //_driverSalaryRepository = driverSalaryRepository;
            _driverViolationRepository = driverViolationRepository;
            _maintenanceRecordRepository = maintenanceRecordRepository;
            _tripExpenseRepository = tripExpenseRepository;
            _logger = loggerFactory.CreateLogger<DriverService>();
            _userRoleRepository = userRoleRepository;
            _tripRepository = tripRepository;
        }

        public async Task<ApiResponse> CreateAsync(CreateDriverDto obj)
        {
            #region Validate
            // Validate Email - Start
            var isExistEmail = await _userRepository.AnyAsync(x => !x.IsDeleted && x.Email.ToLower().Equals(obj.Email.ToLower()));
            if (isExistEmail)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Email),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Email - End

            // Validate Phone Number - Start
            var isExistPhoneNumber = await _userRepository.AnyAsync(x => !x.IsDeleted && x.PhoneNumber != null && x.PhoneNumber.ToLower().Equals(obj.PhoneNumber.ToLower()));
            if (isExistPhoneNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.PhoneNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Phone Number - End

            // Validate License Number - Start
            var licenseNumberRegex = @"^([A-Z]{1,2}\d{6,8}|\d{12})$";
            if (!Regex.IsMatch(obj.LicenseNumber, licenseNumberRegex))
            {
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Số GPLX"),
                    ApiCodeConstants.Common.InvalidFormatMessage
                );
            }
            var isExistLicenseNumber = await _driverRepository
                .AnyAsync(x => !x.IsDeleted && x.LicenseNumber != null && obj.LicenseNumber != null && x.LicenseNumber.ToLower().Equals(obj.LicenseNumber.ToLower()));
            if (isExistLicenseNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.LicenseNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate License Number - End

            // Validate Social Insurance Number - Start
            var socialInsuranceNumberRegex = @"^\d{10}$";
            if (!Regex.IsMatch(obj.SocialInsuranceNumber, socialInsuranceNumberRegex))
            {
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Số BHXH"),
                    ApiCodeConstants.Common.InvalidFormatMessage
                );
            }

            var isExistSocialInsuranceNumber = await _driverRepository
                .AnyAsync(x => !x.IsDeleted && x.SocialInsuranceNumber != null && obj.SocialInsuranceNumber != null && x.SocialInsuranceNumber.ToLower().Equals(obj.SocialInsuranceNumber.ToLower()));
            if (isExistSocialInsuranceNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.SocialInsuranceNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Social Insurance Number - End

            if (!StringHelper.IsValidIdentityNumber(obj.IdentityNumber))
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "CCCD"),
                    ApiCodeConstants.Common.InvalidFormatMessage
                );
            #endregion

            await _userRepository.BeginTransactionAsync();
            try
            {
                var userModel = obj.ToUserEntity();
                await _userRepository.CreateAsync(userModel);
                await _userRepository.SaveChangesAsync();

                var model = obj.ToEntity();
                model.Users.Add(userModel);

                await _driverRepository.CreateAsync(model);

                var userRole = new UserRole
                {
                    RoleId = CommonConstants.Role.DRIVER,
                    UserId = userModel.Id,
                    CreatedDate = DateTime.Now,
                    CreatedBy = obj.CreatedBy
                };
                await _userRoleRepository.CreateAsync(userRole);

                await _driverRepository.SaveChangesAsync();

                await _userRepository.EndTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create driver with message {Message}", ex.Message);

                await _userRepository.RollbackTransactionAsync();
                return ApiResponse.InternalServerError();
            }


            return ApiResponse.Created();
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateDriverDto> objs)
        {
            var model = objs.Select(x => x.ToEntity());

            await _driverRepository.CreateListAsync(model);
            await _driverRepository.SaveChangesAsync();

            return ApiResponse.Created(model.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _driverRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new DriverListDto
                {
                    Id = x.Id,
                    ExperienceYears = x.ExperienceYear,
                    LicenseNumber = x.LicenseNumber,
                    LicenseClass = x.LicenseClass,
                    DriverStatusId = x.DriverStatusId,
                    DriverStatusName = x.DriverStatus.Name,
                    DriverStatusColor = x.DriverStatus.Color,
                    UserId = x.Users.FirstOrDefault().Id,
                    FullName = x.Users.FirstOrDefault().FirstName + " " + x.Users.FirstOrDefault().LastName,
                    Email = x.Users.FirstOrDefault().Email,
                    PhoneNumber = x.Users.FirstOrDefault().PhoneNumber,
                    //AvatarId = x.User.AvatarId,
                    //AvatarUrl = x.User.Avatar == null ? null : _storageService.GetOriginalUrl(x.User.Avatar.FileKey),
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _driverRepository
                .FindByCondition(x => x.Id == id && !x.IsDeleted)
                .Select(x => new DriverDetailDto()
                {
                    Id = x.Id,
                    Username = x.Users.FirstOrDefault().UserName,
                    ExperienceYears = x.ExperienceYear,
                    BaseSalary = x.BaseSalary,
                    LicenseNumber = x.LicenseNumber,
                    LicenseClass = x.LicenseClass,
                    LicenseExpiryDate = x.LicenseExpiryDate,
                    SocialInsuranceNumber = x.SocialInsuranceNumber,
                    //BankBranch = x.BankBranch,
                    //BankNumber = x.BankNumber,
                    EmergencyContactName = x.EmergencyContactName,
                    EmergencyContactPhone = x.EmergencyContactPhone,
                    DriverStatusId = x.DriverStatusId,
                    DriverStatusName = x.DriverStatus.Name,
                    DriverStatusColor = x.DriverStatus.Color,
                    //BankId = x.BankId,
                    UserId = x.Users.FirstOrDefault().Id,
                    FirstName = x.Users.FirstOrDefault().FirstName,
                    LastName = x.Users.FirstOrDefault().LastName,
                    Gender = x.Users.FirstOrDefault().Gender,
                    Email = x.Users.FirstOrDefault().Email,
                    PhoneNumber = x.Users.FirstOrDefault().PhoneNumber,
                    DateOfBirth = x.Users.FirstOrDefault().BirthDay,
                    //IdentityNumber = x.Users.FirstOrDefault().IdentityNumber,
                    //AccessFailedCount = x.User.AccessFailedCount,
                    //LockEnabled = x.User.LockEnabled,
                    //LockEndDate = x.User.LockEndDate,
                    //AvatarId = x.User.AvatarId,
                    //AvatarUrl = x.User.Avatar == null ? null : _storageService.GetOriginalUrl(x.User.Avatar.FileKey),
                    CreatedDate = x.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ApiResponse.NotFound();
            }

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _driverRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new DriverListDto
                {
                    Id = x.Id,
                    ExperienceYears = x.ExperienceYear,
                    LicenseNumber = x.LicenseNumber,
                    LicenseClass = x.LicenseClass,
                    DriverStatusId = x.DriverStatusId,
                    DriverStatusName = x.DriverStatus.Name,
                    DriverStatusColor = x.DriverStatus.Color,
                    UserId = x.Users.FirstOrDefault().Id,
                    FullName = x.Users.FirstOrDefault().FirstName + " " + x.Users.FirstOrDefault().LastName,
                    Email = x.Users.FirstOrDefault().Email,
                    PhoneNumber = x.Users.FirstOrDefault().PhoneNumber,
                    //AvatarId = x.User.AvatarId,
                    //AvatarUrl = x.User.Avatar == null ? null : _storageService.GetOriginalUrl(x.User.Avatar.FileKey),
                    CreatedDate = x.CreatedDate
                });

            var totalRecord = await data.CountAsync();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data
                    .Where(x => (x.ExperienceYears.HasValue && x.ExperienceYears.ToString()!.ToLower().Contains(query.Keyword.ToLower())) ||
                    (x.LicenseNumber.ToLower().Contains(query.Keyword.ToLower())) ||
                    (x.LicenseClass.ToLower().Contains(query.Keyword.ToLower())) ||
                    (x.DriverStatusName.ToLower().Contains(query.Keyword.ToLower())) ||
                    (x.FullName.ToLower().Contains(query.Keyword.ToLower())) ||
                    (x.Email.ToLower().Contains(query.Keyword.ToLower())) ||
                    (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToLower().Contains(query.Keyword.ToLower()))
                );

            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<DriverListDto>
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

        public async Task<ApiResponse> GetPagedAsync(DriverDTParameters parameters)
        {
            var data = await _driverRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _driverRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _driverRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> UpdateAsync(UpdateDriverDto obj)
        {
            var existData = await _driverRepository.GetByIdAsync(obj.Id);

            if (existData == null)
                return ApiResponse.NotFound();

            obj.ToEntity(existData);
            await _driverRepository.UpdateAsync(existData);
            await _driverRepository.SaveChangesAsync();

            return ApiResponse.Success();
        }

        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateDriverDto> objs)
        {
            throw new NotImplementedException();
        }

        //public async Task<ApiResponse> GetAvailableDriversAsync(DriverFilter filter)
        //{
        //    var data = await _driverRepository
        //        .FindByCondition(x => filter.Status == null || filter.Status.Contains(x.DriverStatusId))
        //        .Select(x => new DriverAvailableDto
        //        {
        //            Id = x.Id,
        //            DriverStatusColor = x.DriverStatus.Color,
        //            DriverStatusName = x.DriverStatus.Name,
        //            FullName = x.User.FirstName + " " + x.User.LastName,
        //            PhoneNumber = x.User.PhoneNumber ?? string.Empty,
        //            AvatarUrl = x.User.Avatar == null ? null : _storageService.GetOriginalUrl(x.User.Avatar.FileKey)
        //        })
        //        .ToListAsync();

        //    return ApiResponse.Success(data);
        //}

        public async Task<ApiResponse> GetMyProfileAsync(int userId)
        {
            var driver = await _driverRepository
                .FindByCondition(x => x.Users.FirstOrDefault().Id == userId && !x.IsDeleted)
                .Select(x => new DriverProfileDto
                {
                    Id = x.Id,
                    FullName = x.Users.FirstOrDefault().FirstName + " " + x.Users.FirstOrDefault().LastName,
                    Email = x.Users.FirstOrDefault().Email,
                    PhoneNumber = x.Users.FirstOrDefault().PhoneNumber ?? string.Empty,
                    DriverStatusId = x.DriverStatusId,
                    DriverStatusName = x.DriverStatus.Name,
                    DriverStatusColor = x.DriverStatus.Color,
                    ExperienceYears = x.ExperienceYear,
                    BaseSalary = x.BaseSalary,
                    LicenseNumber = x.LicenseNumber,
                    LicenseClass = x.LicenseClass,
                    LicenseExpiryDate = x.LicenseExpiryDate,
                    SocialInsuranceNumber = x.SocialInsuranceNumber,
                    //BankBranch = x.BankBranch,
                    //BankNumber = x.BankNumber,
                    EmergencyContactName = x.EmergencyContactName,
                    EmergencyContactPhone = x.EmergencyContactPhone,
                    //Avatar = x.User.Avatar == null ? null : new FileUploadDetailDto
                    //{
                    //    Id = x.User.Avatar.Id,
                    //    FileKey = x.User.Avatar.FileKey,
                    //    FileName = x.User.Avatar.FileName,
                    //    FileSize = x.User.Avatar.FileSize,
                    //    FileType = x.User.Avatar.FileType,
                    //    Url = _storageService.GetOriginalUrl(x.User.Avatar.FileKey)
                    //}
                })
                .FirstOrDefaultAsync();

            if (driver == null)
            {
                return ApiResponse.NotFound();
            }

            return ApiResponse.Success(driver);
        }

        public async Task<ApiResponse> UpdateMyProfileAsync(int userId, UpdateDriverProfileDto profileDto)
        {
            if (profileDto == null)
            {
                return ApiResponse.BadRequest();
            }

            var driver = await _driverRepository
                              .FirstOrDefaultAsync(x => x.Users.FirstOrDefault().Id == userId && !x.IsDeleted);
            if (driver == null)
            {
                return ApiResponse.NotFound();
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse.NotFound(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Auth.UserNotFound), ApiCodeConstants.Auth.UserNotFound);
            }

            #region Validate
            var isValidPhoneNumber = PhoneHelper.IsValidVietnamPhone(profileDto.PhoneNumber);
            if (!isValidPhoneNumber)
            {
                return ApiResponse.UnprocessableEntity(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Số điện thoại"),
                   ApiCodeConstants.Common.InvalidData
               );
            }

            var isValidEmail = EmailHelper.IsValidEmail(profileDto.Email);
            if (!isValidEmail)
            {
                return ApiResponse.UnprocessableEntity(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Email"),
                    ApiCodeConstants.Common.InvalidData
                );
            }

            // Validate Email - Start
            var isExistEmail = await _userRepository
                .AnyAsync(x => x.Email.ToLower().Equals(profileDto.Email.ToLower()));
            if (isExistEmail)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", profileDto.Email),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Email - End

            // Validate Phone Number - Start
            var isExistPhoneNumber = await _userRepository
                .AnyAsync(x => x.PhoneNumber != null && x.PhoneNumber.Equals(profileDto.PhoneNumber));
            if (isExistPhoneNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", profileDto.PhoneNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Phone Number - End

            // Validate License Number - Start
            var licenseNumberRegex = @"^([A-Z]{1,2}\d{6,8}|\d{12})$";
            if (!Regex.IsMatch(profileDto.LicenseNumber, licenseNumberRegex))
            {
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Số GPLX"),
                    ApiCodeConstants.Common.InvalidFormatMessage
                );
            }
            var isExistLicenseNumber = await _driverRepository
                .AnyAsync(x => !x.IsDeleted && x.LicenseNumber != null && profileDto.LicenseNumber != null && x.LicenseNumber.ToLower().Equals(profileDto.LicenseNumber.ToLower()));
            if (isExistLicenseNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", profileDto.LicenseNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate License Number - End

            // Validate Social Insurance Number - Start
            var socialInsuranceNumberRegex = @"^\d{10}$";
            if (!Regex.IsMatch(profileDto.SocialInsuranceNumber, socialInsuranceNumberRegex))
            {
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "Số BHXH"),
                    ApiCodeConstants.Common.InvalidFormatMessage
                );
            }

            var isExistSocialInsuranceNumber = await _driverRepository
                .AnyAsync(x => x.SocialInsuranceNumber != null && profileDto.SocialInsuranceNumber != null
                    && x.SocialInsuranceNumber.ToLower().Equals(profileDto.SocialInsuranceNumber.ToLower()));
            if (isExistSocialInsuranceNumber)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", profileDto.SocialInsuranceNumber),
                    ApiCodeConstants.Common.DuplicatedData
                );
            // Validate Social Insurance Number - End

            //if (!StringHelper.IsValidIdentityNumber(profileDto.IdentityNumber))
            //    return ApiResponse.UnprocessableEntity(
            //        ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage).Replace("{PropertyName}", "CCCD"),
            //        ApiCodeConstants.Common.InvalidFormatMessage
            //    );
            #endregion

            using var transaction = await _driverRepository.BeginTransactionAsync();
            try
            {
                user.FirstName = profileDto.FirstName;
                user.LastName = profileDto.LastName;
                user.Email = profileDto.Email;
                user.PhoneNumber = profileDto.PhoneNumber;
                //user.AddresDetail = profileDto.AddressDetail;
                user.Gender = profileDto.GenderId;
                //user.AvatarId = profileDto.AvatarId;
                user.BirthDay = profileDto.DateOfBirth;
                //user.IdentityNumber = profileDto.IdentityNumber;
                user.LastModifiedDate = DateTime.Now;
                user.UpdatedBy = userId;

                driver.ExperienceYear = profileDto.ExperienceYears;
                driver.LicenseNumber = profileDto.LicenseNumber;
                driver.LicenseClass = profileDto.LicenseClass;
                driver.LicenseExpiryDate = profileDto.LicenseExpiryDate;
                driver.SocialInsuranceNumber = profileDto.SocialInsuranceNumber;
                //driver.BankBranch = profileDto.BankBranch;
                //driver.BankNumber = profileDto.BankNumber;
                driver.EmergencyContactName = profileDto.EmergencyContactName;
                driver.EmergencyContactPhone = profileDto.EmergencyContactPhone;
                driver.UpdatedBy = userId;
                driver.LastModifiedDate = DateTime.Now;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                await _driverRepository.UpdateAsync(driver);
                await _driverRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResponse.Success();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return ApiResponse.InternalServerError();
            }

        }

        //public async Task<ApiResponse> GetDriverFuelLogsAsync(FuelLogSearchQuery query)
        //{
        //    var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();
        //    if (driverId.HasValue)
        //    {
        //        query.DriverIds = new List<int> { driverId.Value };
        //    }

        //    var data = await _fuelLogRepository.GetFuelLogsAsync(query);
        //    return ApiResponse.Success(data);
        //}

        //public async Task<ApiResponse> GetDriverSalariesAsync(DriverSalaryFilter filter)
        //{
        //    var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();

        //    if (driverId.HasValue)
        //    {
        //        filter.DriverIds = new List<int> { driverId.Value };
        //    }
        //    var data = await _driverSalaryRepository.GetPagedAsync(filter);
        //    return ApiResponse.Success(data);
        //}

        //public async Task<ApiResponse> GetDriverViolationsAsync(DriverViolationFilter filter)
        //{
        //    var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();

        //    if (driverId.HasValue)
        //    {
        //        filter.DriverIds = new List<int> { driverId.Value };
        //    }

        //    var data = await _driverViolationRepository.GetPagedAsync(filter);
        //    return ApiResponse.Success(data);
        //}

        public async Task<ApiResponse> GetTripExpensesAsync(TripExpenseFilter filter)
        {
            var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();

            if (driverId.HasValue)
            {
                filter.DriverIds = new List<int> { driverId.Value };
            }

            var data = await _tripExpenseRepository.GetPagedAsync(filter);
            return ApiResponse.Success(data);
        }

        //public async Task<ApiResponse> GetMaintenanceRecordsAsync(MaintenanceRecordFilter filter)
        //{
        //    var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();

        //    if (driverId.HasValue)
        //    {
        //        filter.DriverIds = new List<int> { driverId.Value };
        //    }

        //    var data = await _maintenanceRecordRepository.GetPagedAsync(filter);
        //    return ApiResponse.Success(data);
        //}

        public async Task<ApiResponse> GetLicenseClass()
        {
            var data = CommonConstants.LicenseClass
                .Select(x => new DataItem<string>
                {
                    Id = x.Key,
                    Name = x.Value
                })
                .ToList();

            await Task.CompletedTask;
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetTripsAsync(TripFilter filter)
        {
            var driverId = _httpContextAccessor.HttpContext?.GetCurrentDriverId();

            if (driverId.HasValue)
            {
                filter.DriverIds = new List<int> { driverId.Value };
            }

            var data = await _tripRepository.GetPagedAsync(filter);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetDriverAvailableAsync()
        {
            var data = await _driverRepository
                .FindByCondition(x => !x.IsDeleted)
                .Where(d => d.DriverStatusId == CommonConstants.DriverStatus.AVAILABLE)
                .Select(x => new DriverListDto
                {
                    Id = x.Id,
                    ExperienceYears = x.ExperienceYear,
                    LicenseNumber = x.LicenseNumber,
                    LicenseClass = x.LicenseClass,
                    DriverStatusId = x.DriverStatusId,
                    DriverStatusName = x.DriverStatus.Name,
                    DriverStatusColor = x.DriverStatus.Color,
                    UserId = x.Users.FirstOrDefault().Id,
                    FullName = x.Users.FirstOrDefault().FirstName + " " + x.Users.FirstOrDefault().LastName,
                    Email = x.Users.FirstOrDefault().Email,
                    PhoneNumber = x.Users.FirstOrDefault().PhoneNumber,
                    //AvatarId = x.User.AvatarId,
                    //AvatarUrl = x.User.Avatar == null ? null : _storageService.GetOriginalUrl(x.User.Avatar.FileKey),
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            return ApiResponse.Success(data);
        }
    }

}
