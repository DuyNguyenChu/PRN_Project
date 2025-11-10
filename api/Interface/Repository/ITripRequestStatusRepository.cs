using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface ITripRequestStatusRepository : IRepositoryBase<TripRequestStatus, int>
    {
        Task<DTResult<TripRequestStatusAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }

}
