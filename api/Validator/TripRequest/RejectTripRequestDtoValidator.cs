using api.Dtos.TripRequest;
using api.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.TripRequest
{
    public class RejectTripRequestDtoValidator : AbstractValidator<RejectTripRequestDto>
    {
        public RejectTripRequestDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithName("Id")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Id")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

        }
    }
}
