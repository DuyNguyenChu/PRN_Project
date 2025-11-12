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
    public class RejectTripExpenseDtoValidator : AbstractValidator<RejectTripExpenseDto>
    {
        public RejectTripExpenseDtoValidator()
        {
            RuleFor(x => x.Id)
            .NotNull()
            .WithName("Mã chi phí chuyến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã chi phí chuyến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.RejectReason)
            .NotNull()
            .WithName("Lý do từ chối")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Lý do từ chối")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .MaximumLength(1000)
            .WithName("Lý do từ chối")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
        }
    }
}
