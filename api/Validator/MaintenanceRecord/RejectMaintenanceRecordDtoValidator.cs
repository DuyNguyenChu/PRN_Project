using api.Dtos.MaintenanceRecord;
using FluentValidation;

namespace api.Validator.MaintenanceRecord
{
    public class RejectMaintenanceRecordDtoValidator : AbstractValidator<RejectMaintenanceRecordDto>
    {
        public RejectMaintenanceRecordDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");

            RuleFor(x => x.RejectReason)
                .NotNull().NotEmpty().WithMessage("Lí do từ chối là bắt buộc.")
                .MaximumLength(500).WithMessage("Lí do từ chối không quá 500 ký tự.");
        }
    }
}