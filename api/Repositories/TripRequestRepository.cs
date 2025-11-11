using api.DTParameters;
using api.Extensions;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class TripRequestRepository : RepositoryBase<TripRequest, int>, ITripRequestRepository
    {
        private readonly PrnprojectContext _context;
        public TripRequestRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<DTResult<TripRequestAggregate>> GetPagedAsync(TripRequestDTParameters parameters)
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
            var queryTripRequest = _context.TripRequests;
            var queryUser = _context.Users;
            var queryTripRequestStatus = _context.TripRequestStatuses;

            var query = from tr in queryTripRequest
                        join requester in queryUser on tr.RequesterId equals requester.Id
                        join tripRequestStatus in queryTripRequestStatus on tr.TripRequestStatusId equals tripRequestStatus.Id
                        join cancelled in queryUser on tr.RequesterId equals cancelled.Id into cancelledGroup
                        from cancelled in cancelledGroup.DefaultIfEmpty()
                        where !tr.IsDeleted &&
                            !requester.IsDeleted &&
                            !tripRequestStatus.IsDeleted &&
                            (cancelled == null || !cancelled.IsDeleted)
                        select new TripRequestAggregate
                        {
                            CreatedDate = tr.CreatedDate,
                            Description = tr.Description,
                            Id = tr.Id,
                            CancelledByUserId = tr.RequesterId,
                            CancelledName = cancelled != null ? cancelled.FirstName + " " + cancelled.LastName : null,
                            FromLatitude = tr.FromLatitude,
                            FromLocation = tr.FromLocation,
                            FromLongitude = tr.FromLongtitude,
                            // HandledAt = tr.HandledAt,
                            // Purpose = tr.Purpose,
                            // RequestCode = tr.RequestCode,
                            RequesterId = tr.RequesterId,
                            RequesterName = requester.FirstName + " " + requester.LastName,
                            // RejectReason = tr.RejectReason,
                            // RequestedAt = tr.RequestedAt,
                            ToLatitude = tr.ToLatitude,
                            ToLocation = tr.ToLocation,
                            ToLongitude = tr.ToLongtitude,
                            TripRequestStatusId = tr.TripRequestStatusId,
                            TripRequestStatusName = tripRequestStatus.Name,
                            TripRequestStatusColor = tripRequestStatus.Color,
                            // CancelReason = tr.CancelReason,
                            // ExpectedStartTime = tr.ExpectedStartTime,
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x =>
                    (x.Description != null && EF.Functions.Collate(x.Description.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.CancelledName != null && EF.Functions.Collate(x.CancelledName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.TripRequestStatusName != null && EF.Functions.Collate(x.TripRequestStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    //EF.Functions.Collate(x.RequestCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.RequesterName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
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
                    //case "requestCode":
                    //    query = query
                    //        .Where(r => EF.Functions.Collate(r.RequestCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                    //    break;
                    case "fromLocation":
                        query = query
                            .Where(r => EF.Functions.Collate(r.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                        break;
                    case "toLocation":
                        query = query
                            .Where(r => EF.Functions.Collate(r.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                        break;
                    case "description":
                        query = query.Where(r => r.Description != null && EF.Functions.Collate(r.Description.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                        break;
                    case "tripRequestStatusName":
                        var statusIds = search.Split(",");
                        query = query.Where(x => statusIds.Contains(x.TripRequestStatusId.ToString()));
                        //query = query.Where(r => r.TripRequestStatusName != null && EF.Functions.Collate(r.TripRequestStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
                        break;
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

            if (parameters.TripRequestStatusIds.Any())
            {
                query = query.Where(x => parameters.TripRequestStatusIds.Contains(x.TripRequestStatusId));
            }
            if (parameters.RequesterIds.Any())
            {
                query = query.Where(x => parameters.RequesterIds.Contains(x.RequesterId));
            }
            if (parameters.CancellerIds.Any())
            {
                query = query.Where(x => x.CancelledByUserId != null && parameters.CancellerIds.Contains((int)x.CancelledByUserId));
            }

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
            var data = new DTResult<TripRequestAggregate>
            {
                draw = parameters.Draw,
                data = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync(),
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;
        }

        public async Task<PagingData<TripRequestTripAggregate>> GetPagedAsync(TripRequestFilter filter)
        {
            var keyword = filter.Keyword;

            var query = from tr in _context.TripRequests
                        join requester in _context.Users on tr.RequesterId equals requester.Id
                        join tripRequestStatus in _context.TripRequestStatuses on tr.TripRequestStatusId equals tripRequestStatus.Id
                        join trip in _context.Trips on tr.Id equals trip.TripRequestId into tripGroup
                        from trip in tripGroup.DefaultIfEmpty()
                        join tripStatus in _context.TripStatuses on trip.TripStatusId equals tripStatus.Id into tripStatusGroup
                        from tripStatus in tripStatusGroup.DefaultIfEmpty()
                        join vehicle in _context.Vehicles on trip.VehicleId equals vehicle.Id into vehicleGroup
                        from vehicle in vehicleGroup.DefaultIfEmpty()
                        join vehicleModel in _context.VehicleModels on vehicle.VehicleModelId equals vehicleModel.Id into vehicleModelGroup
                        from vehicleModel in vehicleModelGroup.DefaultIfEmpty()
                        join vehicleBrand in _context.VehicleBranches on vehicle.VehicleBranchId equals vehicleBrand.Id into vehicleBrandGroup
                        from vehicleBrand in vehicleBrandGroup.DefaultIfEmpty()
                        join driverUser in _context.Users on trip.DriverId equals driverUser.Id into driverUserGroup
                        from driverUser in driverUserGroup.DefaultIfEmpty()
                        where !tr.IsDeleted &&
                            !requester.IsDeleted &&
                            !tripRequestStatus.IsDeleted &&
                            (trip == null || !trip.IsDeleted)
                        select new TripRequestTripAggregate
                        {
                            Description = tr.Description,
                            Id = tr.Id,
                            FromLocation = tr.FromLocation,
                            // HandledAt = tr.HandledAt,
                            // Purpose = tr.Purpose,
                            // RequestCode = tr.RequestCode,
                            RequesterId = tr.RequesterId,
                            RequesterName = requester.FirstName + " " + requester.LastName,
                            RequesterPhone = requester.PhoneNumber,
                            // RejectReason = tr.RejectReason,
                            // RequestedAt = tr.RequestedAt,
                            ToLocation = tr.ToLocation,
                            TripRequestStatusId = tr.TripRequestStatusId,
                            TripRequestStatusName = tripRequestStatus.Name,
                            TripRequestStatusColor = tripRequestStatus.Color,
                            // CancelReason = tr.CancelReason,
                            TripId = trip == null ? null : trip.Id,
                            // TripCode = trip == null ? null : trip.TripCode,
                            // DispatchTime = trip == null ? null : trip.DispatchTime,
                            // ConfirmationTime = trip == null ? null : trip.ConfirmationTime,
                            PickUpTime = trip == null ? null : trip.PickUpTime,
                            ActualStartTime = trip == null ? null : trip.ActualStartTime,
                            ActualEndTime = trip == null ? null : trip.ActualEndTime,
                            // CompletionTime = trip == null ? null : trip.CompletionTime,
                            // CancellationTime = trip == null ? null : trip.CancellationTime,
                            DriverId = trip == null ? null : trip.DriverId,
                            DriverPhone = trip == null ? null : driverUser.PhoneNumber,
                            DriverName = trip == null ? null : driverUser.FirstName + " " + driverUser.LastName,
                            // Notes = trip == null ? null : trip.Notes,
                            TripStatusId = trip == null ? null : trip.TripStatusId,
                            TripStatusName = trip == null ? null : tripStatus.Name,
                            TripStatusColor = trip == null ? null : tripStatus.Color,
                            VehicleId = trip == null ? null : trip.VehicleId,
                            // VehicleLicensePlate = trip == null ? null : vehicle.,
                            VehicleBrandName = trip == null ? null : vehicleBrand.Name,
                            VehicleModelName = trip == null ? null : vehicleModel.Name,
                        };

            if (filter.RequesterIds.Any())
            {
                query = query
                    .Where(x => filter.RequesterIds.Contains(x.RequesterId));
            }

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x =>
                    // (x.RejectReason != null && EF.Functions.Collate(x.RejectReason.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    // (x.CancelReason != null && EF.Functions.Collate(x.CancelReason.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.Description != null && EF.Functions.Collate(x.Description.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.TripRequestStatusName != null && EF.Functions.Collate(x.TripRequestStatusName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    // x.HandledAt.HasValue && x.HandledAt.Value.ToVietnameseDateTimeOffset().Contains(keyword) ||
                    //EF.Functions.Collate(x.RequestCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.FromLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.ToLocation.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    // EF.Functions.Collate(x.Purpose.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.RequesterName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))
                    // x.RequestedAt.ToVietnameseDateTimeOffset().Contains(keyword)
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
                    // query = query.Where(c => c.RequestedAt >= startDate && c.RequestedAt <= endDate);
                }
                else
                {
                    var date = DateTime.ParseExact(filter.PeriodTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    // query = query.Where(c => c.RequestedAt.Date == date.Date);
                }
            }

            if (filter.TripStatusIds.Any())
            {
                query = query
                    .Where(x => x.TripStatusId != null && filter.TripStatusIds.Contains(x.TripStatusId.Value));
            }

            if (filter.TripRequestStatusIds.Any())
            {
                query = query
                    .Where(x => filter.TripRequestStatusIds.Contains(x.TripRequestStatusId));
            }

            var totalFiltered = await query.CountAsync();

            if (string.IsNullOrEmpty(filter.OrderBy))
            {
                // query = query
                //     .OrderByDescending(x => x.RequestedAt);
            }
            else
            {
                query = query.OrderByDynamic(filter.OrderBy, filter.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripRequestTripAggregate>
            {
                CurrentPage = filter.PageIndex,
                PageSize = filter.PageSize,
                DataSource = await query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = totalFiltered
            };

            return pagedData;
        }
    }
}
