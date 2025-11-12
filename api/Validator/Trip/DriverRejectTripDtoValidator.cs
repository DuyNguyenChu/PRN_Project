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
    public class DriverRejectTripDtoValidator : AbstractValidator<DriverRejectTripDto>
    {
        public DriverRejectTripDtoValidator()
        {
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