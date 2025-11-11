using api.Dtos.TripStatus;
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
    public class TripStatusService : ITripStatusService
    {
        private readonly ITripStatusRepository _tripStatusRepository;

        public TripStatusService(ITripStatusRepository tripStatusRepository)
        {
            _tripStatusRepository = tripStatusRepository;
        }
        public async Task<ApiResponse> CreateAsync(CreateTripStatusDto obj)
        {
            var isExistName = await _tripStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower());
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _tripStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color == obj.Color);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var model = obj.ToEntity();
            await _tripStatusRepository.CreateAsync(model);
            await _tripStatusRepository.SaveChangesAsync();
            return ApiResponse.Created(model);
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateTripStatusDto> objs)
        {
            var models = objs.Select(x => x.ToEntity());

            await _tripStatusRepository.CreateListAsync(models);
            await _tripStatusRepository.SaveChangesAsync();

            return ApiResponse.Created(models.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _tripStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripStatusListDto()
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
            var data = await _tripStatusRepository
                .FindByCondition(x => x.Id == id && !x.IsDeleted)
                .Select(x => new TripStatusDetailDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? "",
                    Color = x.Color,
                    CreatedDate = x.CreatedDate,
                }).FirstOrDefaultAsync();
            if (data == null)
            {
                return ApiResponse.NotFound();
            }
            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _tripStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new TripStatusListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? "",
                    Color = x.Color,
                    CreatedDate = x.CreatedDate
                });

            var totalRecord = await data.CountAsync();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                data = data
                    .Where(x => x.Name.ToLower().Contains(query.Keyword.ToLower()) ||
                    x.Description != null && x.Description.ToLower().Contains(query.Keyword.ToLower()) ||
                    x.Color.ToLower().Contains(query.Keyword.ToLower()) ||
                    x.CreatedDate.ToVietnameseDateTimeOffset().Contains(query.Keyword)
                );

            }
            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                data = data
                       .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripStatusListDto>
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
            var data = await _tripStatusRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _tripStatusRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _tripStatusRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public async Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            var isDeleted = await _tripStatusRepository.SoftDeleteListAsync(objs);
            if (!isDeleted)
                return ApiResponse.BadRequest();
            await _tripStatusRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public async Task<ApiResponse> UpdateAsync(UpdateTripStatusDto obj)
        {
            var isExistName = await _tripStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower().Equals(obj.Name.ToLower()) && x.Id != obj.Id);
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _tripStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color.Equals(obj.Color) && x.Id != obj.Id);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var existObj = await _tripStatusRepository
                .FindByCondition(x => x.Id == obj.Id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (existObj == null)
            {
                return ApiResponse.NotFound(obj, "Not found");
            }

            var model = obj.ToEntity(existObj);
            await _tripStatusRepository.UpdateAsync(model);
            await _tripStatusRepository.SaveChangesAsync();
            return ApiResponse.Success(model);
        }

        public async Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateTripStatusDto> objs)
        {
            var listIds = objs.Select(x => x.Id).ToList();

            var existData = await _tripStatusRepository
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

            await _tripStatusRepository.UpdateListAsync(existData);
            await _tripStatusRepository.SaveChangesAsync();

            return ApiResponse.Success(existData);
        }
    }
}
