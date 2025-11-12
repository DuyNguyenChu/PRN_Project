using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Driver;
using api.Dtos.FuelLog;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Dtos.DriverStatus;

namespace api.Interface.Services
{
    public interface IDriverStatusService : IServiceBase<int, CreateDriverStatusDto, UpdateDriverStatusDto, api.Extensions.DTParameters>
    {
    }
}
