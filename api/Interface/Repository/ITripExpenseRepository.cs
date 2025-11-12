using api.DTParameters;
using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface ITripExpenseRepository : IRepositoryBase<TripExpense, int>
    {
        Task<PagingData<TripExpenseAggregate>> GetPagedAsync(TripExpenseFilter filter);
        Task<DTResult<TripExpenseAggregate>> GetPagedAsync(TripExpenseDTParameters parameters);
    }
}
