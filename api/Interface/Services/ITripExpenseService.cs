using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripExpense;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface ITripExpenseService : IServiceBase<int, CreateTripExpenseDto, UpdateTripExpenseDto, TripExpenseDTParameters>
    {
        Task<ApiResponse> GetPagedAsync(TripExpenseDTParameters parameters);
        Task<ApiResponse> CreateListAsync(int tripId, IEnumerable<TripCreateTripExpenseDto> objs);
        Task<ApiResponse> ApproveAsync(ApproveTripExpenseDto obj);
        Task<ApiResponse> RejectAsync(RejectTripExpenseDto obj);
        Task<ApiResponse> GetStatus();
    }
}
