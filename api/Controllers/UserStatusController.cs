﻿using System;
using api.Dtos.UserStatus;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatusController : BaseController, IBaseController<int, CreateUserStatusDto, UpdateUserStatusDto, api.Extensions.DTParameters>
    {
        private readonly IUserStatusService _userStatusService;
        public UserStatusController(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }

        [HttpPost]
        //[CustomAuthorize(Enums.Menu.USER_STATUS, Enums.Action.CREATE)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserStatusDto obj)
        {
            var userId = this.GetLoggedInUserId();
            obj.CreatedBy = userId;
            var result = await _userStatusService.CreateAsync(obj);
            return BaseResult(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _userStatusService.GetAllAsync();
            return BaseResult(result);
        }

        //[CustomAuthorize(Enums.Menu.USER_STATUS, Enums.Action.READ)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _userStatusService.GetByIdAsync(id);
            return BaseResult(result);
        }


        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var result = await _userStatusService.GetPagedAsync(query);
            return BaseResult(result);
        }

        //[CustomAuthorize(Enums.Menu.USER_STATUS, Enums.Action.READ)]
        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var result = await _userStatusService.GetPagedAsync(parameters);
            return BaseResult(result);
        }

        //[CustomAuthorize(Enums.Menu.USER_STATUS, Enums.Action.DELETE)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var result = await _userStatusService.SoftDeleteAsync(id);
            return BaseResult(result);
        }

        [HttpPut]
        //[CustomAuthorize(Enums.Menu.USER_STATUS, Enums.Action.UPDATE)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserStatusDto obj)
        {
            var userId = this.GetLoggedInUserId();
            obj.UpdatedBy = userId;
            var result = await _userStatusService.UpdateAsync(obj);
            return BaseResult(result);
        }

    }
}
