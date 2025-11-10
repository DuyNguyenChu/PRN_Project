using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface ITripStatusRepository : IRepositoryBase<TripStatus, int>
    {
        public Task<DTResult<TripStatusAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }
}
