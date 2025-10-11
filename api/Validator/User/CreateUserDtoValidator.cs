﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Extensions;
using FluentValidation;
using api.Helpers;

namespace api.Validator.User
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.PasswordHash)
                .NotNull()
                .WithName("Mật khẩu")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Mật khẩu")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(64)
                .WithName("Mật khẩu")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .MinimumLength(8)
                .WithName("Mật khẩu")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MinLengthMessage));

            RuleFor(x => x.FirstName)
                .NotNull()
                .WithName("Họ và tên đệm")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Họ và tên đệm")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(255)
                .WithName("Họ và tên đệm")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

            RuleFor(x => x.LastName)
                .NotNull()
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(255)
                .WithName("Tên")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

            RuleFor(x => x.Email)
                .NotNull()
                .WithName("Email")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Email")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(500)
                .WithName("Email")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50)
                .WithName("số điện thoại")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || PhoneHelper.IsValidVietnamPhone(phoneNumber))
                .WithName("Số điện thoại")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidFormatMessage));

            RuleFor(x => x.Gender)
                .Must(gender => gender == null || gender == (int)Gender.Male || gender == (int)Gender.Female)
                .WithName("Giới tính")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData));
        }
    }
}
