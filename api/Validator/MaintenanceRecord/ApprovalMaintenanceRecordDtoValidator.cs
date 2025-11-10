using api.Dtos.MaintenanceRecord;
using FluentValidation;

namespace api.Validator.MaintenanceRecord
{
    public class ApprovalMaintenanceRecordDtoValidator : AbstractValidator<ApprovalMaintenanceRecordDto>
    {
        public ApprovalMaintenanceRecordDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");
        }
    }
}