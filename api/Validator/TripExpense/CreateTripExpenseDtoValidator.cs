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
    public class CreateTripExpenseDtoValidator : AbstractValidator<CreateTripExpenseDto>
    {
        public CreateTripExpenseDtoValidator()
        {
            RuleFor(x => x.TripId)
            .NotNull()
            .WithName("Mã chuyến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã chuyến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.ExpenseTypeId)
            .NotNull()
            .WithName("Mã loại chi phí")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã loại chi phí")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.Amount)
            .NotNull()
            .WithName("Số tiền")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Số tiền")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .GreaterThan(0)
            .WithName("Số tiền")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanValueMessage));

            RuleFor(x => x.ExpenseDate)
            .NotNull()
            .WithName("Thời gian phát sinh")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Thời gian phát sinh")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .LessThan(DateTime.Now)
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.LessThanValueMessage));


            RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithName("Ghi chú")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
        }
    }
}
