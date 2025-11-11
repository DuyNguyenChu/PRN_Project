using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripRequest;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface ITripRequestService : IServiceBase<int, CreateTripRequestDto, UpdateTripRequestDto, TripRequestDTParameters>
    {
        Task<ApiResponse> RejectAsync(RejectTripRequestDto obj);
        Task<ApiResponse> CancelAsync(CancelTripRequestDto obj);
        Task<ApiResponse> ApproveAsync(ApproveTripRequestDto obj);
    }
}
