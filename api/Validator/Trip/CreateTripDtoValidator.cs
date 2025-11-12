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
    public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(x => x.VehicleId)
            .NotNull()
            .WithName("Mã xe")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã xe")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.DriverId)
            .NotNull()
            .WithName("Mã tài xế")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Mã tài xế")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.FromLocation)
            .MaximumLength(500)
            .WithName("Tên địa điểm đi")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
            .NotNull()
            .WithName("Tên địa điểm đi")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Tên địa điểm đi")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.ToLocation)
            .MaximumLength(500)
            .WithName("Tên địa điểm đến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
            .NotNull()
            .WithName("Tên địa điểm đến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
            .NotEmpty()
            .WithName("Tên địa điểm đến")
            .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
        }
    }
}
