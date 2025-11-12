using api.Dtos.TripExpense;
using api.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.Trip
{
    public class ApproveTripExpenseDtoValidator : AbstractValidator<ApproveTripExpenseDto>
    {
        public ApproveTripExpenseDtoValidator()
        {
            RuleFor(x => x.Id)
           .NotNull()
           .WithName("Mã chi phí chuyến")
           .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
           .NotEmpty()
           .WithName("Mã chi phí chuyến")
           .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
        }
    }
}
