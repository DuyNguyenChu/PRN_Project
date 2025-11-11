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

            RuleFor(x => x.ScheduledStartTime)
               .NotNull()
               .WithName("Thời gian dự kiến xuất phát")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("Thời gian dự kiến xuất phát")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .LessThan(x => x.ScheduledEndTime)
               .WithName("Thời gian dự kiến xuất phát")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.LessThanMessage));

            RuleFor(x => x.ScheduledEndTime)
               .NotNull()
               .WithName("Thời gian dự kiến hoàn thành")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .NotEmpty()
               .WithName("Thời gian dự kiến hoàn thành")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.RequiredMessage))
               .GreaterThan(x => x.ScheduledStartTime)
               .WithName("Thời gian dự kiến hoàn thành")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.GreaterThanMessage));

            RuleFor(x => x.Notes)
               .MaximumLength(1000)
               .WithName("Ghi chú")
               .WithMessage(ErrorMessagesConstants.GetMessage(ApiCodeConstants.Common.MaxLengthMessage));
        }
    }
}
