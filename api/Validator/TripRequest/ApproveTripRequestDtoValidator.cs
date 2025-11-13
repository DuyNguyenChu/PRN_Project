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
    public class ApproveTripRequestDtoValidator : AbstractValidator<ApproveTripRequestDto>
    {
        public ApproveTripRequestDtoValidator()
        {
            RuleFor(x => x.Id)
               .NotNull()
               .WithName("Mã yêu cầu")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("Mã yêu cầu")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.DriverId)
               .NotNull()
               .WithName("Tài xế")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("Tài xế")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.VehicleId)
               .NotNull()
               .WithName("Xe")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("Xe")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
        }
    }
}
