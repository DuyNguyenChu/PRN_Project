using api.Dtos.UserStatus;
using api.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.UserStatus
{
    public class UpdateUserStatusDtoValidator : AbstractValidator<UpdateUserStatusDto>
    {
        public UpdateUserStatusDtoValidator()
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
