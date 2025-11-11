using api.Dtos.TripRequestStatus;
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
    public class TripRequestStatusService : ITripRequestStatusService
    {
        private readonly ITripRequestStatusRepository _tripRequestStatusRepository;
        public TripRequestStatusService(ITripRequestStatusRepository tripRequestStatusRepository)
        {
            _tripRequestStatusRepository = tripRequestStatusRepository;
        }
        public async Task<ApiResponse> CreateAsync(CreateTripRequestStatusDto obj)
        {
            var isExistName = await _tripRequestStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower());
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _tripRequestStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color == obj.Color);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var model = obj.ToEntity();
            await _tripRequestStatusRepository.CreateAsync(model);
            await _tripRequestStatusRepository.SaveChangesAsync();
            return ApiResponse.Created(model);
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateTripRequestStatusDto> objs)
        {
            var models = objs.Select(x => x.ToEntity());

            await _tripRequestStatusRepository.CreateListAsync(models);
            await _tripRequestStatusRepository.SaveChangesAsync();

            return ApiResponse.Created(models.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _tripRequestStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripRequestStatusListDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? "",
                    Color = x.Color,
                    CreatedDate = x.CreatedDate
                }).ToListAsync();
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _tripRequestStatusRepository
                .FindByCondition(x => x.Id == id && !x.IsDeleted)
                .Select(x => new TripRequestStatusDetailDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? "",
                    Color = x.Color,
                    CreatedDate = x.CreatedDate
                }).FirstOrDefaultAsync();
            if (data == null)
            {
                return ApiResponse.NotFound();
            }
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _tripRequestStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripRequestStatusListDto
                {
                    Color = x.Color,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name
                });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data
                    .Where(x => x.Name.ToLower().Contains(query.Keyword.ToLower()) ||
                    x.Description != null && x.Description.ToLower().Contains(query.Keyword.ToLower())
                );
            }
            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = query.SortType.ToLower().Equals("asc") ? data.OrderByDynamic(query.OrderBy, LinqExtensions.Order.Asc) : data.OrderByDynamic(query.OrderBy, LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripRequestStatusListDto>
            {
                CurrentPage = query.PageIndex,
                PageSize = query.PageSize,
                DataSource = await data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await data.CountAsync()
            };

            return ApiResponse.Success(pagedData);
        }

        // public Task<ApiResponse> GetPagedAsync<T>(AdvancedSearchQuery<T> query)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<ApiResponse> GetPagedAsync(api.Extensions.DTParameters parameters)
        {
            var data = await _tripRequestStatusRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _tripRequestStatusRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _tripRequestStatusRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public async Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            var isDeleted = await _tripRequestStatusRepository.SoftDeleteListAsync(objs);
            if (!isDeleted)
                return ApiResponse.BadRequest();
            return ApiResponse.Success(isDeleted);
        }

        public async Task<ApiResponse> UpdateAsync(UpdateTripRequestStatusDto obj)
        {
            var isExistName = await _tripRequestStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower() && x.Id != obj.Id);
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _tripRequestStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color == obj.Color && x.Id != obj.Id);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var existObj = await _tripRequestStatusRepository
                .FindByCondition(x => x.Id == obj.Id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (existObj == null)
            {
                return ApiResponse.NotFound(obj, "Not found");
            }

            obj.ToEntity(existObj);
            await _tripRequestStatusRepository.UpdateAsync(existObj);
            await _tripRequestStatusRepository.SaveChangesAsync();
            return ApiResponse.Success(existObj);
        }

        public async Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateTripRequestStatusDto> objs)
        {
            var listIds = objs.Select(x => x.Id).ToList();

            var existData = await _tripRequestStatusRepository
                .FindByCondition(x => listIds.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync();

            if (existData.Count != listIds.Count)
            {
                return ApiResponse.BadRequest();
            }

            foreach (var item in objs)
            {
                var existObj = existData.Find(x => x.Id == item.Id);
                if (existObj != null)
                {
                    item.ToEntity(existObj);
                }
            }

            await _tripRequestStatusRepository.UpdateListAsync(existData);
            await _tripRequestStatusRepository.SaveChangesAsync();

            return ApiResponse.Success(existData);
        }
    }
}
