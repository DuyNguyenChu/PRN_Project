using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.TripStatus;
using api.Extensions;

namespace api.Interface.Services
{
    public interface ITripStatusService : IServiceBase<int, CreateTripStatusDto, UpdateTripStatusDto, api.Extensions.DTParameters>
    {
    }
}
