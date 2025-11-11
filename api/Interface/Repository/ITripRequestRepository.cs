using api.DTParameters;
using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface ITripRequestRepository : IRepositoryBase<TripRequest, int>
    {
        Task<DTResult<TripRequestAggregate>> GetPagedAsync(TripRequestDTParameters parameters);
        Task<PagingData<TripRequestTripAggregate>> GetPagedAsync(TripRequestFilter filter);
    }
}
