using api.Dtos.Trip;
using api.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.Trip
{
    public class DriverUpdateTripCompleteDtoValidator : AbstractValidator<DriverUpdateTripCompleteDto>
    {
        public DriverUpdateTripCompleteDtoValidator()
        {
            RuleFor(x => x.EndOdometer)
           .NotNull()
           .WithName("Số km trên công tơ mét")
           .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
           .NotEmpty()
           .WithName("Số km trên công tơ mét")
           .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
           .GreaterThan(0)
           .WithName("Số km trên công tơ mét")
           .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanValueMessage));

            RuleFor(x => x.TripId)
            .NotNull()
            .WithName("Mã chuyến xe")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã chuyến xe")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
        }
    }
}
