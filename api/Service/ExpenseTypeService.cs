using api.Dtos.ExpenseType;
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
    public class ExpenseTypeService : IExpenseTypeService
    {
        private readonly IExpenseTypeRepository _expenseTypeRepository;

        public ExpenseTypeService(IExpenseTypeRepository expenseTypeRepository)
        {
            _expenseTypeRepository = expenseTypeRepository;
        }

        public async Task<ApiResponse> CreateAsync(CreateExpenseTypeDto obj)
        {
            var isExistName = await _expenseTypeRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower());
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var model = obj.ToEntity();

            await _expenseTypeRepository.CreateAsync(model);
            await _expenseTypeRepository.SaveChangesAsync();

            return ApiResponse.Created(model.Id);
        }

        public async Task<ApiResponse> CreateListAsync(IEnumerable<CreateExpenseTypeDto> objs)
        {
            var model = objs.Select(x => x.ToEntity());

            await _expenseTypeRepository.CreateListAsync(model);
            await _expenseTypeRepository.SaveChangesAsync();

            return ApiResponse.Created(model.Select(x => x.Id));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var data = await _expenseTypeRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new ExpenseTypeDetailDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    Name = x.Name,
                })
                .ToListAsync();

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var data = await _expenseTypeRepository.GetByIdAsync(id);
            if (data == null)
                return ApiResponse.NotFound();

            var dto = data.ToDto();

            return ApiResponse.Success(dto);
        }

        public async Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            var data = _expenseTypeRepository
                .FindByCondition(x => !x.IsDeleted)
                .Select(x => new ExpenseTypeListDto
                {
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

            var pagedData = new PagingData<ExpenseTypeListDto>
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
            var data = await _expenseTypeRepository.GetPagedAsync(parameters);

            return ApiResponse.Success(data);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _expenseTypeRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.BadRequest();

            await _expenseTypeRepository.SaveChangesAsync();
            return ApiResponse.Success(isDeleted);
        }

        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> UpdateAsync(UpdateExpenseTypeDto obj)
        {
            var isExistName = await _expenseTypeRepository.AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == obj.Name.ToLower() && x.Id != obj.Id);
            if (isExistName)
                return ApiResponse.UnprocessableEntity(
                    ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", obj.Name),
                    ApiCodeConstants.Common.DuplicatedData
                );
            var existData = await _expenseTypeRepository.GetByIdAsync(obj.Id);
            if (existData == null)
                return ApiResponse.NotFound();

            obj.ToEntity(existData);

            await _expenseTypeRepository.UpdateAsync(existData);
            await _expenseTypeRepository.SaveChangesAsync();

            return ApiResponse.Success();
        }

        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdateExpenseTypeDto> obj)
        {
            throw new NotImplementedException();
        }
    }
}
