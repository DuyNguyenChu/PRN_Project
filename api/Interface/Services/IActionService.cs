using api.Dtos.Action;
using api.Extensions;

namespace api.Interface.Services
{
    public interface IActionService : IServiceBase<int, CreateActionDto, UpdateActionDto, api.Extensions.DTParameters>
    {
    }
}
