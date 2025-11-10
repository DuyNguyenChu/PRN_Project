using api.Dtos.FuelLog;
using FluentValidation;

namespace api.Validator.FuelLog
{
    public class ApprovalFuelLogDtoValidator : AbstractValidator<ApprovalFuelLogDto>
    {
        public ApprovalFuelLogDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");
        }
    }
}