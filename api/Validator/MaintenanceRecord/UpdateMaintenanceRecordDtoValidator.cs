using api.Dtos.MaintenanceRecord;
using FluentValidation;
using api.Helpers;
using System;

namespace api.Validator.MaintenanceRecord
{
    public class UpdateMaintenanceRecordDtoValidator : AbstractValidator<UpdateMaintenanceRecordDto>
    {
        public UpdateMaintenanceRecordDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");

            RuleFor(x => x.VehicleId)
                .GreaterThan(0).WithMessage("Xe là bắt buộc.");

            RuleFor(x => x.Odometer)
                .GreaterThan(0).WithMessage("Số Odometer phải lớn hơn 0.");

            RuleFor(x => x.ServiceType)
                .NotNull().NotEmpty().WithMessage("Loại dịch vụ là bắt buộc.")
                .Must(serviceType => CommonConstants.ServiceType.ContainsKey(serviceType))
                .WithMessage("Loại dịch vụ không hợp lệ.");

            RuleFor(x => x.ServiceProvider)
                .NotNull().NotEmpty().WithMessage("Nơi bảo dưỡng là bắt buộc.")
                .MaximumLength(255).WithMessage("Nơi bảo dưỡng không quá 255 ký tự.");

            RuleFor(x => x.StartTime)
                .NotNull().WithMessage("Ngày bắt đầu là bắt buộc.")
                .Must(startTime => startTime <= DateTimeOffset.Now)
                .WithMessage("Ngày bắt đầu không được vượt quá hiện tại.");

            RuleFor(x => x.EndTime)
                .Must((dto, endTime) => endTime == null || endTime >= dto.StartTime)
                .WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Ghi chú không quá 500 ký tự.");

            RuleFor(x => x.Details)
                .NotNull().NotEmpty().WithMessage("Chi tiết sửa chữa là bắt buộc.");

            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(d => d.Description)
                    .NotNull().NotEmpty().WithMessage("Nội dung chi tiết là bắt buộc.")
                    .MaximumLength(500).WithMessage("Nội dung chi tiết không quá 500 ký tự.");

                detail.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("Đơn giá phải lớn hơn 0.");

                detail.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
            });
        }
    }
}