using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface IUserStatusRepository : IRepositoryBase<UserStatus, int>
    {
        Task<DTResult<UserStatusAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }

}
