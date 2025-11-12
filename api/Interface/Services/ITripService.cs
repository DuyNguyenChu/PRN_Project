using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Trip;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface ITripService : IServiceBase<int, CreateTripDto, UpdateTripDto, TripDTParameters>
    {
        Task<ApiResponse> GetPagedAsync(TripDTParameters parameters);
        //Task<ApiResponse> RequesterCancelTrip(CancelTripDto obj);
        Task<ApiResponse> DriverAcceptTrip(DriverUpdateTripDto obj);
        Task<ApiResponse> DriverRejectTrip(DriverRejectTripDto obj);
        Task<ApiResponse> DriverMovingToPickup(DriverUpdateTripDto obj);
        Task<ApiResponse> DriverArrivedAtPickup(DriverUpdateTripDto obj);
        Task<ApiResponse> DriverMovingToDestination(DriverUpdateTripMovingToDestinationDto obj);
        Task<ApiResponse> DriverArrivedAtDestination(DriverUpdateTripDto obj);
        Task<ApiResponse> DriverCompleteTrip(DriverUpdateTripCompleteDto obj);
        Task<ApiResponse> CancelAsync(CancelTripDto obj);
    }
}
