using api.DTParameters;
using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface ITripRepository : IRepositoryBase<Trip, int>
    {
        Task<DTResult<TripAggregate>> GetPagedAsync(TripDTParameters parameters);
        Task<PagingData<TripAggregate>> GetPagedAsync(TripFilter filter);
    }
}
