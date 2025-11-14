using api.Dtos.DriverStatus;
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
    public class DriverStatusService : IDriverStatusService
    {
        private readonly IDriverStatusRepository _driverStatusRepository;

        public DriverStatusService(IDriverStatusRepository DriverStatusRepository)
        {
            _driverStatusRepository = DriverStatusRepository;
        }

        public async Task<ApiResponse> CreateAsync(CreateDriverStatusDto obj)
        {
            var isExistName = await _driverStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower());
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _driverStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color == obj.Color);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var model = obj.ToEntity();

            await _driverStatusRepository.CreateAsync(model);
            await _driverStatusRepository.SaveChangesAsync();

            return ApiResponse.Created(model.Id);
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateDriverStatusDto> objs)
        {
            var models = objs.Select(x => x.ToEntity());

            await _driverStatusRepository.CreateListAsync(models);
            await _driverStatusRepository.SaveChangesAsync();

            return ApiResponse.Created(models.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _driverStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new DriverStatusDetailDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Color = x.Color,
                    CreatedDate = x.CreatedDate,
                })
                .ToListAsync();

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _driverStatusRepository.GetByIdAsync(id);
            if (data == null)
                return ApiResponse.NotFound();

            var dto = data.ToDto();

            return ApiResponse.Success(dto);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _driverStatusRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new DriverStatusListDto
                {
                    Color = x.Color,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
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
                data = data
                    .OrderByDynamic(query.OrderBy, query.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<DriverStatusListDto>
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

        public async Task<ApiResponse> GetPagedAsync(api.Extensions.DTParameters parameters)
        {
            var data = await _driverStatusRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _driverStatusRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _driverStatusRepository.SaveChangesAsync();

            return ApiResponse.Success();
        }

        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> UpdateAsync(UpdateDriverStatusDto obj)
        {
            var isExistName = await _driverStatusRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower() && x.Id != obj.Id);
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var isExistColor = await _driverStatusRepository.AnyAsync(x => !x.IsDeleted && x.Color == obj.Color && x.Id != obj.Id);
            if (isExistColor)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Color),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var existData = await _driverStatusRepository.GetByIdAsync(obj.Id);
            if (existData == null)
                return ApiResponse.NotFound();

            obj.ToEntity(existData);

            await _driverStatusRepository.UpdateAsync(existData);
            await _driverStatusRepository.SaveChangesAsync();

            return ApiResponse.Success();
        }

        public async Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateDriverStatusDto> obj)
        {
            var listIds = obj.Select(x => x.Id).ToList();

            var existData = await _driverStatusRepository
                .FindByConditionAsync(x => !x.IsDeleted && listIds.Contains(x.Id));

            if (listIds.Count != existData.Count)
                return ApiResponse.BadRequest();

            foreach (var item in obj)
            {
                var existObj = existData.Find(x => x.Id == item.Id);
                if (existObj != null)
                    item.ToEntity(existObj);
            }

            await _driverStatusRepository.UpdateListAsync(existData);
            await _driverStatusRepository.SaveChangesAsync();

            return ApiResponse.Success();
        }
    }
}
