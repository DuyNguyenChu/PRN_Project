using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.ExpenseType;
using api.DTParameters;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IExpenseTypeService : IServiceBase<int, CreateExpenseTypeDto, UpdateExpenseTypeDto, api.Extensions.DTParameters>
    {
    }
}
