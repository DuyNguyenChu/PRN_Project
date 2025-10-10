using api.Dtos.Action;
using api.Extensions;
using api.Interface.Services;

namespace api.Service
{
    public interface IActionService : IServiceBase<int, CreateActionDto, UpdateActionDto, DTParameters>
    {
    }
}
