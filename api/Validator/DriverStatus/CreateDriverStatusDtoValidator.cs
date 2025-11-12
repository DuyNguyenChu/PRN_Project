using api.Dtos.DriverStatus;
using api.Dtos.Trip;
using api.Extensions;
using api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.DriverStatus
{
    internal class CreateDriverStatusDtoValidator : AbstractValidator<CreateDriverStatusDto>
    {
        public CreateDriverStatusDtoValidator()
        {

            RuleFor(x => x.Name)
                .NotNull()
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(255)
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithName("Mô tả")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            RuleFor(x => x.Color)
                .NotNull()
                .WithName("Màu sắc")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Màu sắc")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(50)
                .WithName("Màu sắc")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

        }
    }
}
