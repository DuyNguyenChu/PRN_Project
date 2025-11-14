using System;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.ViolationType;

namespace api.Controllers
{
    [Route("api/violation-type")]
    [ApiController]
    public class ViolationTypeController : BaseController, IBaseController<int, CreateViolationTypeDto, UpdateViolationTypeDto, api.Extensions.DTParameters>
    {
        private readonly IViolationTypeService _violationTypeService;
        public ViolationTypeController(IViolationTypeService violationTypeService)
        {
            _violationTypeService = violationTypeService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateViolationTypeDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _violationTypeService.CreateAsync(obj);

            return BaseResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _violationTypeService.GetAllAsync();

            return BaseResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _violationTypeService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _violationTypeService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var data = await _violationTypeService.GetPagedAsync(parameters);

            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _violationTypeService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateViolationTypeDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _violationTypeService.UpdateAsync(obj);

            return BaseResult(data);
        }
    }
}
