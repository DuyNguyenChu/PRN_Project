using api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Helpers
{
    public interface IBaseController<TKey, CreateDto, UpdateDto, AdvancedDTParameter>
        where CreateDto : class
        where UpdateDto : class
        where AdvancedDTParameter : api.Extensions.DTParameters
    {
        Task<IActionResult> GetByIdAsync(TKey id);
        Task<IActionResult> CreateAsync([FromBody] CreateDto obj);
        Task<IActionResult> UpdateAsync([FromBody] UpdateDto obj);
        Task<IActionResult> SoftDeleteAsync(TKey id);
        Task<IActionResult> GetAllAsync();
        Task<IActionResult> GetPagedAsync([FromBody] SearchQuery query);
        Task<IActionResult> GetPagedAsync([FromBody] AdvancedDTParameter parameters);
    }

}
