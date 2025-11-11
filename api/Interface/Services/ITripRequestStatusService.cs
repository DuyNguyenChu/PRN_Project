using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripRequestStatus;
using api.Extensions;

namespace api.Interface.Services
{
    public interface ITripRequestStatusService : IServiceBase<int, CreateTripRequestStatusDto, UpdateTripRequestStatusDto, api.Extensions.DTParameters>
    {

    }
}
