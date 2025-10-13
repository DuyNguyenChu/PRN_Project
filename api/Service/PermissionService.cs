﻿using api.Dtos.Permission;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionService(IPermissionRepository permissionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _permissionRepository = permissionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse> CreateAsync(CreatePermissionDto obj)
        {
            var isExist = await _permissionRepository
                .AnyAsync(x => !x.IsDeleted && x.ActionId == obj.ActionId && x.MenuId == obj.MenuId && x.RoleId == obj.RoleId);
            if (isExist)
                return ApiResponse.UnprocessableEntity(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.DuplicatedData).Replace("{key}", ""),
                    ApiCodeConstants.Common.DuplicatedData);

            var model = obj.ToEntity();
            await _permissionRepository.CreateAsync(model);
            await _permissionRepository.SaveChangesAsync();

            return ApiResponse.Success(model.Id);
        }

        public Task<ApiResponse> CreateListAsync(IEnumerable<CreatePermissionDto> objs)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetPagedAsync(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        //public Task<ApiResponse> GetPagedAsync<T>(AdvancedSearchQuery<T> query)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<ApiResponse> GetPagedAsync(api.Extensions.DTParameters parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var isDeleted = await _permissionRepository.SoftDeleteAsync(id);
            if (!isDeleted)
                return ApiResponse.NotFound();

            await _permissionRepository.SaveChangesAsync();

            return ApiResponse.Success(isDeleted);
        }

        public Task<ApiResponse> SoftDeleteListAsync(IEnumerable<int> objs)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateAsync(UpdatePermissionDto obj)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateListAsync(IEnumerable<UpdatePermissionDto> obj)
        {
            throw new NotImplementedException();
        }

    }
}
