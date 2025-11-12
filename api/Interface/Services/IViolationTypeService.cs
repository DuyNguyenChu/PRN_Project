using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.ViolationType;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IViolationTypeService : IServiceBase<int, CreateViolationTypeDto, UpdateViolationTypeDto, api.Extensions.DTParameters>
    {
    }
}
