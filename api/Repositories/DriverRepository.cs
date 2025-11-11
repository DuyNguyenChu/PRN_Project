using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repositories
{
    public class DriverRepository : RepositoryBase<Driver, int>, IDriverRepository
    {
        private readonly PrnprojectContext _context;

        public DriverRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }
        public async Task<DTResult<DriverAggregate>> GetPagedAsync(DriverDTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;
            if (parameters.Order != null && parameters.Order.Any())
            {
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() == "asc";
            }
            else
            {
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }

            var query = from driver in _context.Drivers
                        join driverStatus in _context.DriverStatuses on driver.DriverStatusId equals driverStatus.Id
                        // *** PHẦN SỬA LỖI ***
                        // 1. Bỏ join 'user' cũ
                        // 2. Dùng 'let' để lấy user đầu tiên không bị xóa từ danh sách Users của Driver
                        let user = driver.Users.FirstOrDefault(u => !u.IsDeleted)

                        // 3. Join 'avatar' dựa trên 'user' từ mệnh đề 'let'
                        //    Chúng ta phải kiểm tra 'user' null trước khi lấy 'user.AvatarId'
                        join fileUpload in _context.FileUploads.Where(x => !x.IsDeleted)
                            on (user == null ? (int?)null : user.AvatarId) equals fileUpload.Id into userAvatar
                        from avatar in userAvatar.DefaultIfEmpty()

                            // 4. Điều chỉnh 'where'. Chỉ lấy 'driver' có 'user' hợp lệ (user != null)
                            //    Điều này thay thế logic của INNER JOIN cũ và 'where !user.IsDeleted'
                        where (!driver.IsDeleted && !driverStatus.IsDeleted && user != null)
                        // *** KẾT THÚC SỬA LỖI ***
                        select new DriverAggregate
                        {
                            Id = driver.Id,
                            ExperienceYears = driver.ExperienceYear,
                            LicenseNumber = driver.LicenseNumber,
                            LicenseClass = driver.LicenseClass,
                            DriverStatusId = driver.DriverStatusId,
                            DriverStatusName = driverStatus.Name,
                            DriverStatusColor = driverStatus.Color,
                            UserId = user.Id,
                            FullName = user.FirstName + " " + user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            AvatarId = user.AvatarId,
                            // AvatarUrl = avatar != null ? _storageService.GetOriginalUrl(avatar.FileKey) : null,
                            CreatedDate = driver.CreatedDate
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x => EF.Functions.Collate(x.LicenseNumber.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        (x.ExperienceYears.HasValue) && EF.Functions.Collate(x.ExperienceYears.ToString()!, SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.LicenseClass.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.DriverStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.FullName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.Email.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        (!string.IsNullOrEmpty(x.PhoneNumber) && EF.Functions.Collate(x.PhoneNumber.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                     );
            }
            foreach (var column in parameters.Columns)
            {
                var search = column.Search.Value;
                if (!search.Contains("/"))
                {
                    search = column.Search.Value.ToLower();
                }
                if (string.IsNullOrEmpty(search)) continue;
                switch (column.Data)
                {
                    case "createdDate":
                        if (search.Contains(" - "))
                        {
                            var dates = search.Split(" - ");
                            var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query = query.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate);
                        }
                        else
                        {
                            var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            query = query.Where(c => c.CreatedDate.Date == date.Date);
                        }
                        break;
                }
            }
            if (parameters.ExperienceYears.Any())
            {
                query = query.Where(r => parameters.ExperienceYears.Any(s => s.Equals(r.ExperienceYears)));
            }
            if (parameters.LicenseClasses.Any())
            {
                query = query.Where(r => parameters.LicenseClasses.Any(s => s.Equals(r.LicenseClass)));
            }
            if (parameters.DriverStatusIds.Any())
            {
                query = query.Where(r => parameters.DriverStatusIds.Contains(r.DriverStatusId));
            }

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var records = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();
            records.ForEach(item => item.LicenseClassName = CommonConstants.LicenseClass.GetValueOrDefault(item.LicenseClass) ?? string.Empty);

            var data = new DTResult<DriverAggregate>
            {
                draw = parameters.Draw,
                data = records,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;
        }
    }

}
