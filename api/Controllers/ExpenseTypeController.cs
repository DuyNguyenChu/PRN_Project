using System;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.ExpenseType;

namespace api.Controllers
{
    [Route("api/expense-type")]
    [ApiController]
    public class ExpenseTypeController : BaseController, IBaseController<int, CreateExpenseTypeDto, UpdateExpenseTypeDto, api.Extensions.DTParameters>
    {
        private readonly IExpenseTypeService _expenseTypeService;
        public ExpenseTypeController(IExpenseTypeService expenseTypeService)
        {
            _expenseTypeService = expenseTypeService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateExpenseTypeDto obj)
        {
            obj.CreatedBy = this.GetLoggedInUserId();
            var result = await _expenseTypeService.CreateAsync(obj);

            return BaseResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _expenseTypeService.GetAllAsync();

            return BaseResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _expenseTypeService.GetByIdAsync(id);

            return BaseResult(data);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query)
        {
            var data = await _expenseTypeService.GetPagedAsync(query);

            return BaseResult(data);
        }

        [HttpPost("paged-advanced")]
        public async Task<IActionResult> GetPagedAsync([FromBody] api.Extensions.DTParameters parameters)
        {
            var data = await _expenseTypeService.GetPagedAsync(parameters);

            return BaseResult(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var data = await _expenseTypeService.SoftDeleteAsync(id);

            return BaseResult(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateExpenseTypeDto obj)
        {
            obj.UpdatedBy = this.GetLoggedInUserId();
            var data = await _expenseTypeService.UpdateAsync(obj);

            return BaseResult(data);
        }
    }
}
