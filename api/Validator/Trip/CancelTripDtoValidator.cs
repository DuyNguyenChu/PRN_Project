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
    public class CancelTripDtoValidator : AbstractValidator<CancelTripDto>
    {
        public CancelTripDtoValidator()
        {
            RuleFor(x => x.TripId)
                .NotNull()
                .WithName("Mã chuyến đi")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Mã chuyến đi")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
        }
    }
}
