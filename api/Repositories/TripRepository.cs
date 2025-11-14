using System.Globalization;
using api.DTParameters;
using api.Extensions;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class TripRepository : RepositoryBase<Trip, int>, ITripRepository
    {
        private readonly PrnprojectContext _context;
        public TripRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }
        public async Task<DTResult<TripAggregate>> GetPagedAsync(TripDTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;
            if (parameters.Order != null && parameters.Order.Length > 0)
            {
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() != "desc";
            }
            else
            {
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }

            var query = from t in _context.Trips
                        join ts in _context.TripStatuses on t.TripStatusId equals ts.Id
                        join v in _context.Vehicles on t.VehicleId equals v.Id
                        join vm in _context.VehicleModels on v.VehicleModelId equals vm.Id
                        join vb in _context.VehicleBranches on v.VehicleBranchId equals vb.Id
                        join d in _context.Drivers on t.DriverId equals d.Id
                        join creator in _context.Users on t.CreatedBy equals creator.Id
                        join approvalUser in _context.Users on t.UpdatedBy equals approvalUser.Id into approvalGroup
                        from approval in approvalGroup.DefaultIfEmpty()
                        join tr in _context.TripRequests on t.TripRequestId equals tr.Id into tripRequestGroup
                        from tr in tripRequestGroup.DefaultIfEmpty()
                        where !t.IsDeleted
                               && !ts.IsDeleted
                               && !v.IsDeleted
                               && !vm.IsDeleted
                               && !vb.IsDeleted
                               && !d.IsDeleted
                               && (t.TripRequestId == null || !tr.IsDeleted)
                               && !creator.IsDeleted
                               && (t.UpdatedBy == null || !approval.IsDeleted)
                        select new TripAggregate
                        {
                            Id = t.Id,
                            // TripCode = t.TripCode,
                            TripStatusId = t.TripStatusId,
                            TripStatusName = ts.Name,
                            TripStatusColor = ts.Color,
                            RequesterId = tr != null ? tr.RequesterId : null,
                            RequesterName = tr != null ? tr.Requester.FirstName + " " + tr.Requester.LastName : null,
                            RequesterPhone = tr != null ? tr.Requester.PhoneNumber : null,
                            VehicleId = t.VehicleId,
                            // VehicleLicensePlate = v.LicensePlate,
                            VehicleModelName = vm.Name,
                            VehicleBrandName = vb.Name,
                            DriverId = d.Id,
                            DriverName = d.Users.FirstOrDefault().FirstName + " " + d.Users.FirstOrDefault().LastName,
                            DriverPhone = d.Users.FirstOrDefault().PhoneNumber,
                            FromLocation = t.FromLocation,
                            FromLatitude = t.FromLatitude,
                            FromLongitude = t.FromLongtitude,
                            ToLocation = t.ToLocation,
                            ToLatitude = t.ToLatitude,
                            ToLongitude = t.ToLongtitude,
                            // Distance = t.Distance,
                            // Purpose = t.Purpose,
                            // Notes = t.Notes,
                            // ScheduledStartTime = t.ScheduledStartTime,
                            // ScheduledEndTime = t.ScheduledEndTime,
                            ActualStartTime = t.ActualStartTime,
                            ActualEndTime = t.ActualEndTime,
                            CreatedBy = t.CreatedBy ?? 0,
                            CreatedByName = creator.FirstName + " " + creator.LastName,
                            ApprovalBy = t.UpdatedBy,
                            ApprovalByName = approval.FirstName + " " + approval.LastName,
                            CreatedDate = t.CreatedDate
                            // CancelReason = t.CancelReason,
                            // RejectReason = t.RejectReason
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x =>
                        // (EF.Functions.Collate(x.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.TripStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.RequesterName != null && EF.Functions.Collate(x.RequesterName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.RequesterPhone != null && EF.Functions.Collate(x.RequesterPhone.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                         // (EF.Functions.Collate(x.VehicleLicensePlate.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                         (EF.Functions.Collate(x.VehicleBrandName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                          (EF.Functions.Collate(x.VehicleModelName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.DriverPhone != null && EF.Functions.Collate(x.DriverPhone.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (EF.Functions.Collate(x.Distance.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (EF.Functions.Collate(x.Purpose.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (x.Notes != null && EF.Functions.Collate(x.Notes.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.CreatedByName.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.ApprovalByName.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
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
                    // case "tripCode":
                    //     query = query
                    //         .Where(r => EF.Functions.Collate(r.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                    //     break;
                    case "fromLocation":
                        query = query
.Where(r => EF.Functions.Collate(r.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)) ||
                            EF.Functions.Collate(r.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General))
                            );
                        break;
                    // case "scheduledStartTime":
                    //     if (search.Contains(" - "))
                    //     {
                    //         var dates = search.Split(" - ");
                    //         var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //         var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    //         query = query.Where(c => c.ScheduledStartTime >= startDate && c.ScheduledStartTime <= endDate);
                    //     }
                    //     else
                    //     {
                    //         var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //         query = query.Where(c => c.ScheduledStartTime.Date == date.Date);
                    //     }
                    //     break;
                    //case "approvalByName":
                    //    query = query
                    //        .Where(r => EF.Functions.Collate(r.ApprovalByName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                    //    break;
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

            if (parameters.RequesterIds.Any())
            {
                query = query.Where(x => x.RequesterId.HasValue && parameters.RequesterIds.Contains(x.RequesterId.Value));
            }
            if (parameters.VehicleIds.Any())
            {
                query = query.Where(x => parameters.VehicleIds.Contains(x.VehicleId));
            }
            if (parameters.DriverIds.Any())
            {
                query = query.Where(x => parameters.DriverIds.Contains(x.DriverId));
            }
            if (parameters.TripStatusIds.Any())
            {
                query = query.Where(x => parameters.TripStatusIds.Contains(x.TripStatusId));
            }

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var data = new DTResult<TripAggregate>
            {
                draw = parameters.Draw,
                data = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync(),
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;
        }

        public async Task<PagingData<TripAggregate>> GetPagedAsync(TripFilter filter)
        {
            var keyword = filter.Keyword;

            var query = from t in _context.Trips
                        join ts in _context.TripStatuses on t.TripStatusId equals ts.Id
                        join v in _context.Vehicles on t.VehicleId equals v.Id
                        join vm in _context.VehicleModels on v.VehicleModelId equals vm.Id
                        join vb in _context.VehicleBranches on v.VehicleBranchId equals vb.Id
                        join d in _context.Drivers on t.DriverId equals d.Id
                        join creator in _context.Users on t.CreatedBy equals creator.Id
                        join approval in _context.Users on t.UpdatedBy equals approval.Id
                        join tr in _context.TripRequests on t.TripRequestId equals tr.Id into tripRequestGroup
                        from tr in tripRequestGroup.DefaultIfEmpty()
                        where !t.IsDeleted
                               && !ts.IsDeleted
                               && !v.IsDeleted
                               && !vm.IsDeleted
                               && !vb.IsDeleted
                               && !d.IsDeleted
                               && (t.TripRequestId == null || !tr.IsDeleted)
                               && !creator.IsDeleted
                               && !approval.IsDeleted
                        select new TripAggregate
                        {
                            Id = t.Id,
                            // TripCode = t.TripCode,
                            TripStatusId = t.TripStatusId,
                            TripStatusName = ts.Name,
                            TripStatusColor = ts.Color,
                            RequesterId = tr != null ? tr.RequesterId : null,
                            RequesterName = tr != null ? tr.Requester.FirstName + " " + tr.Requester.LastName : null,
                            RequesterPhone = tr != null ? tr.Requester.PhoneNumber : null,
                            VehicleId = t.VehicleId,
                            // VehicleLicensePlate = v.LicensePlate,
                            VehicleModelName = vm.Name,
                            VehicleBrandName = vb.Name,
                            DriverId = d.Id,
                            DriverName = d.Users.FirstOrDefault().FirstName + " " + d.Users.FirstOrDefault().LastName,
                            DriverPhone = d.Users.FirstOrDefault().PhoneNumber,
                            FromLocation = t.FromLocation,
                            FromLatitude = t.FromLatitude,
                            FromLongitude = t.FromLongtitude,
                            ToLocation = t.ToLocation,
                            ToLatitude = t.ToLatitude,
                            ToLongitude = t.ToLongtitude,
                            // Distance = t.Distance,
                            // Purpose = t.Purpose,
                            // Notes = t.Notes,
                            // ScheduledStartTime = t.ScheduledStartTime,
                            // ScheduledEndTime = t.ScheduledEndTime,
                            ActualStartTime = t.ActualStartTime,
                            ActualEndTime = t.ActualEndTime,
                            CreatedBy = t.CreatedBy ?? 0,
                            CreatedByName = creator.FirstName + " " + creator.LastName,
                            ApprovalBy = t.UpdatedBy,
                            ApprovalByName = approval.FirstName + " " + approval.LastName,
                            CreatedDate = t.CreatedDate
                            // CancelReason = t.CancelReason,
                            // RejectReason = t.RejectReason
                        };

            if (filter.DriverIds.Any())
            {
                query = query.Where(x => filter.DriverIds.Contains(x.DriverId));
            }

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x =>
                        // (EF.Functions.Collate(x.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.TripStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.RequesterName != null && EF.Functions.Collate(x.RequesterName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.RequesterPhone != null && EF.Functions.Collate(x.RequesterPhone.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
// (EF.Functions.Collate(x.VehicleLicensePlate.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
(EF.Functions.Collate(x.VehicleBrandName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                          (EF.Functions.Collate(x.VehicleModelName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (x.DriverPhone != null && EF.Functions.Collate(x.DriverPhone.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (EF.Functions.Collate(x.Distance.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (EF.Functions.Collate(x.Purpose.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        // (x.Notes != null && EF.Functions.Collate(x.Notes.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.CreatedByName.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        (EF.Functions.Collate(x.ApprovalByName.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                    );
            }

            if (!string.IsNullOrEmpty(filter.PeriodTime))
            {
                filter.PeriodTime = filter.PeriodTime.Replace(" ", "");
                if (filter.PeriodTime.Contains("-"))
                {
                    var dates = filter.PeriodTime.Split("-");
                    var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    query = query.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate);
                }
                else
                {
                    var date = DateTime.ParseExact(filter.PeriodTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(c => c.CreatedDate.Date == date.Date);
                }
            }

            if (filter.RequesterIds.Any())
            {
                query = query.Where(x => x.RequesterId.HasValue && filter.RequesterIds.Contains(x.RequesterId.Value));
            }
            if (filter.VehicleIds.Any())
            {
                query = query.Where(x => filter.VehicleIds.Contains(x.VehicleId));
            }

            if (filter.TripStatusIds.Any())
            {
                query = query.Where(x => filter.TripStatusIds.Contains(x.TripStatusId));
            }

            var totalFiltered = await query.CountAsync();

            if (string.IsNullOrEmpty(filter.OrderBy))
            {
                query = query
                    .OrderByDescending(x => x.CreatedDate);
            }
            else
            {
                query = query.OrderByDynamic(filter.OrderBy, filter.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripAggregate>
            {
                CurrentPage = filter.PageIndex,
                PageSize = filter.PageSize,
                DataSource = await query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = totalFiltered
            };

            return pagedData;
        }


        // public async Task<DTResult<DistanceDetailByRegionAggregate>> GetDistanceDetailByRegionIdAsync(DistanceDetailByRegionDTParameters parameters)
        // {
        //     var keyword = parameters.Search?.Value;
        //     var orderCriteria = string.Empty;
        //     var orderAscendingDirection = true;
        //     if (parameters.Order != null && parameters.Order.Length > 0)
        //     {
        //         orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
        //         orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() != "asc";
        //     }
        //     else
        //     {
        //         orderCriteria = "tripId";
        //         orderAscendingDirection = true;
        //     }
        //     var baseQuery = from t in _context.Trips
        //                     join v in _context.Vehicles on t.VehicleId equals v.Id
        //                     join d in _context.Drivers on t.DriverId equals d.Id
        //                     join du in _context.Users on d.UserId equals du.Id
        //                     join r in _context.Regions on t.RegionId equals r.Id
        //                     join vm in _context.VehicleModels on v.VehicleModelId equals vm.Id
        //                     join vb in _context.VehicleBrands on v.VehicleBrandId equals vb.Id
        //                     join tr in _context.TripRequests on t.TripRequestId equals tr.Id into trg
        //                     from tr in trg.DefaultIfEmpty()
        //                         //join trs in _context.TripRequestStatuses on tr.TripRequestStatusId equals trs.Id
        //                     join ru in _context.Users on tr.RequesterId equals ru.Id into rug
        //                     from ru in rug.DefaultIfEmpty()
        //                     join o in _context.Offices on t.OfficeId equals o.Id into og
        //                     from org in og.DefaultIfEmpty()
        //                     where !t.IsDeleted && !v.IsDeleted && !vm.IsDeleted && !vb.IsDeleted && !d.IsDeleted
        //                     && tr.TripRequestStatusId == CommonConstants.TripRequestStatus.APPROVED
        //                     && t.ActualEndTime.HasValue && t.ActualStartTime.HasValue && t.StartOdometer.HasValue && t.EndOdometer.HasValue
        //                     select new
        //                     {
        //                         Trip = t,
        //                         Vehicle = v,
        //                         Driver = d,
        //                         DriverUser = du,
        //                         Region = r,
        //                         VehicleModel = vm,
        //                         VehicleBrand = vb,
        //                         TripRequest = tr,
        //                         Requester = ru,
        //                         Office = org
        //                     };

        //     var query = baseQuery.Select(b => new DistanceDetailByRegionAggregate
        //     {
        //         TripId = b.Trip.Id,
        //         TripCode = b.Trip.TripCode,
        //         RegionId = b.Trip.RegionId,
        //         RegionName = b.Region.Name,
        //         OfficeName = b.Office.Name,
        //         DriverId = b.Driver.Id,
        //         DriverName = b.DriverUser.FirstName + " " + b.DriverUser.LastName,
        //         VehicleBrandName = b.VehicleBrand.Name,
        //         VehicleName = b.VehicleModel.Name,
        //         VehicleId = b.Vehicle.Id,
        //         LicensePlate = b.Vehicle.LicensePlate,
        //         FromLocation = b.Trip.FromLocation,
        //         ToLocation = b.Trip.ToLocation,
        //         StartTime = b.Trip.ActualStartTime,
        //         EndTime = b.Trip.ActualEndTime,
        //         Duration = b.Trip.ActualEndTime.HasValue && b.Trip.ActualStartTime.HasValue ? b.Trip.ActualEndTime.Value - b.Trip.ActualStartTime.Value : TimeSpan.Zero,
        //         MovingDate = b.Trip.ScheduledStartTime,
        //         KmFee = CommonConstants.Fare.REVENUE,
        //         KmPrice = b.Trip.Revenue,
        //         TotalFee = b.Trip.Revenue,
        //         StartOdometer = b.Trip.StartOdometer,
        //         Distance = b.Trip.Distance,
        //         EndOdometer = b.Trip.EndOdometer,
        //         TotalFeeWithVat = b.Trip.Revenue / (1 - CommonConstants.Fare.VAT_RATE_REVENUE),
        //         UserId = b.Requester.Id,
        //         UserUseName = b.Requester.FirstName + " " + b.Requester.LastName,
        //         CreatedDate = b.Trip.CreatedDate,
        //         ExpenseDetails = new List<DistanceDetailByRegionAggregate.ExpenseDetail>(),
        //         Description = b.TripRequest.Description ?? string.Empty,
        //     });


        //     var totalRecord = await query.CountAsync();

        //     if (!string.IsNullOrEmpty(keyword))
        //     {
        //         keyword = keyword.ToLower();
        //         query = query
        //             .Where(x =>
        //                 (EF.Functions.Collate(x.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.RegionName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.VehicleName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.Distance.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                  x.StartTime.HasValue && x.StartTime.Value.ToVietnameseDateTimeOffset().Contains(keyword) ||
        //                  x.EndTime.HasValue && x.EndTime.Value.ToVietnameseDateTimeOffset().Contains(keyword) ||
        //                 //(EF.Functions.Collate(x.MovingDate.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 //(EF.Functions.Collate(x.Trip.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.KmFee.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.KmPrice.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.TotalFee.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.TotalFeeWithVat.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                 (EF.Functions.Collate(x.UserUseName.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
        //                    x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
        //                 );

        //     }

        //     foreach (var column in parameters.Columns)
        //     {
        //         var search = column.Search.Value;
        //         if (!search.Contains("/"))
        //         {
        //             search = column.Search.Value.ToLower();
        //         }
        //         if (string.IsNullOrEmpty(search)) continue;
        //         switch (column.Data)
        //         {
        //             case "tripCode":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "regionName":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.RegionName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "driverName":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "vehicleName":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.VehicleName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "fromLocation":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)) ||
        //                     EF.Functions.Collate(r.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General))
        //                     );
        //                 break;
        //             case "startTime":
        //                 if (search.Contains(" - "))
        //                 {
        //                     var dates = search.Split(" - ");
        //                     var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        //                     query = query.Where(c => c.StartTime >= startDate && c.StartTime <= endDate);
        //                 }
        //                 else
        //                 {
        //                     var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     query = query.Where(c => c.StartTime == date.Date);
        //                 }
        //                 break;
        //             case "endTime":
        //                 if (search.Contains(" - "))
        //                 {
        //                     var dates = search.Split(" - ");
        //                     var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        //                     query = query.Where(c => c.EndTime >= startDate && c.EndTime <= endDate);
        //                 }
        //                 else
        //                 {
        //                     var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     query = query.Where(c => c.EndTime == date.Date);
        //                 }
        //                 break;
        //             case "totalFee":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.TotalFee.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "totalFeeWithVat":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.TotalFeeWithVat.ToString().ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "userUseName":
        //                 query = query
        //                     .Where(r => EF.Functions.Collate(r.UserUseName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
        //                 break;
        //             case "createdDate":
        //                 if (search.Contains(" - "))
        //                 {
        //                     var dates = search.Split(" - ");
        //                     var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
        //                     query = query.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate);
        //                 }
        //                 else
        //                 {
        //                     var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //                     query = query.Where(c => c.CreatedDate.Date == date.Date);
        //                 }
        //                 break;

        //         }

        //     }
        //     if (parameters.RegionIds.Any())
        //     {
        //         query = query.Where(x => parameters.RegionIds.Contains(x.RegionId));
        //     }

        //     if (parameters.DriverIds.Any())
        //     {
        //         query = query.Where(x => parameters.DriverIds.Contains(x.DriverId));
        //     }


        //     if (parameters.UserIds.Any())
        //     {
        //         query = query.Where(x => parameters.UserIds.Contains(x.UserId));
        //     }

        //     query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
        //     var records = parameters.IsExportExcel || parameters.Length == -1 ? await query.ToListAsync() : await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();


        //     // Lấy expense types và trip expenses
        //     var expenseDetails = await _context.ExpenseTypes
        //         .AsNoTracking()
        //         .Select(et => new
        //         {
        //             Id = et.Id,
        //             Name = et.Name,
        //             CreatedDate = et.CreatedDate,
        //             TripExpenses = et.TripExpenses
        //                 .Where(te => !te.IsDeleted)
        //                 .Select(te => new { te.TripId, te.Amount })
        //         })
        //         .ToListAsync();

        //     // Tạo dictionary sau khi đã lấy dữ liệu về
        //     var expenseDetailsWithDictionary = expenseDetails.Select(et => new
        //     {
        //         et.Id,
        //         et.Name,
        //         et.CreatedDate,
        //         TripExpenses = et.TripExpenses
        //             .GroupBy(te => te.TripId)
        //             .ToDictionary(
        //                 g => g.Key,
        //                 g => g.Sum(x => x.Amount)
        //             )
        //     });

        //     // Map expense details cho từng record
        //     foreach (var record in records)
        //     {
        //         record.Trip = $"{record.FromLocation} - {record.ToLocation}";
        //         record.ExpenseDetails = expenseDetailsWithDictionary
        //             .Select(et => new DistanceDetailByRegionAggregate.ExpenseDetail
        //             {
        //                 Id = et.Id,
        //                 Name = et.Name,
        //                 Amount = et.TripExpenses.TryGetValue(record.TripId, out var amount) ? amount : 0,
        //                 CreatedDate = et.CreatedDate
        //             })
        //             .ToList();
        //     }
        //     var data = new DTResult<DistanceDetailByRegionAggregate>
        //     {
        //         draw = parameters.Draw,
        //         data = records,
        //         recordsFiltered = await query.CountAsync(),
        //         recordsTotal = totalRecord
        //     };

        //     return data;
        // }
    }
}
