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
    public class CreateTripRequestDtoValidator : AbstractValidator<CreateTripRequestDto>
    {
        public CreateTripRequestDtoValidator()
        {

            RuleFor(x => x.FromLocation)
                .NotNull()
                .WithName("Điểm xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Điểm xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(500)
                .WithName("Điểm xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

            RuleFor(x => x.FromLatitude)
                .NotNull()
                .WithName("Vĩ độ xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Vĩ độ xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.FromLongitude)
                .NotNull()
                .WithName("Kinh độ xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Kinh độ xuất phát")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.ToLocation)
                .NotNull()
                .WithName("Điếm đến")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Điếm đến")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .MaximumLength(500)
                .WithName("Điếm đến")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

            RuleFor(x => x.ToLatitude)
                .NotNull()
                .WithName("Vĩ độ đích")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Vĩ độ đích")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));

            RuleFor(x => x.ToLongitude)
                .NotNull()
                .WithName("Kinh độ đích")
.WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
                .NotEmpty()
                .WithName("Kinh độ đích")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage));


            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithName("Mô tả")
                .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));

        }
    }
}
