using api.Dtos.Driver;
using api.Dtos.Trip;
using api.Extensions;
using api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.Driver
{
    public class CreateDriverDtoValidator : AbstractValidator<CreateDriverDto>
    {
        public CreateDriverDtoValidator()
        {
            RuleFor(x => x.BaseSalary)
                .NotNull()
                .WithName("Lương cơ bản")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Lương cơ bản")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .GreaterThan(0)
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanValueMessage));
            RuleFor(x => x.LicenseNumber)
                .NotNull()
                .WithName("Số giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Số giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(255)
                .WithName("Số giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            RuleFor(x => x.LicenseClass)
                .NotNull()
                .WithName("Hạng giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Hạng giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(50)
                .WithName("Hạng giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .Must(licenseClass => CommonConstants.LicenseClass.ContainsKey(licenseClass))
                .WithName("Hạng giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData));
            RuleFor(x => x.LicenseExpiryDate)
                .NotNull()
                .WithName("Thời hạn giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Thời hạn giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .Must(licenseExpiryDate => licenseExpiryDate > DateTime.Now)
                .WithName("Thời hạn giấy phép lái xe")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData));
            RuleFor(x => x.SocialInsuranceNumber)
                .NotNull()
                .WithName("Số BHXH")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Số BHXH")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(50)
                .WithName("Số BHXH")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            //RuleFor(x => x.BankBranch)
            //    .MaximumLength(255)
            //    .WithName("Chi nhánh ngân hàng")
            //    .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            //RuleFor(x => x.BankNumber)
            //    .MaximumLength(50)
            //    .WithName("Số tài khoản")
            //    .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            RuleFor(x => x.EmergencyContactName)
                .MaximumLength(255)
                .WithName("Tên người liên hệ khẩn cấp")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
            RuleFor(x => x.EmergencyContactPhone)
                .MaximumLength(50)
                .WithName("Số điện thoại liên hệ khẩn cấp")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .Must(emergencyContactPhone => PhoneHelper.IsValidVietnamPhone(emergencyContactPhone))
                .WithName("Số điện thoại liên hệ khẩn cấp")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData))
                .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));
            RuleFor(x => x.DriverStatusId)
                .NotNull()
                .WithName("Trạng thái tài xế")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Trạng thái tài xế")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));
            RuleFor(x => x.Password)
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
            RuleFor(x => x.IdentityNumber)
               .NotNull()
               .WithName("CCCD")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("CCCD")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .MaximumLength(255)
               .WithName("CCCD")
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
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .Must(email => EmailHelper.IsValidEmail(email))
                .WithName("Email")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData)); ;
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50)
                .WithName("Số điện thoại")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage))
                .Must(phoneNumber => PhoneHelper.IsValidVietnamPhone(phoneNumber))
                .WithName("Số điện thoại")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.InvalidData));
        }
    }

}
