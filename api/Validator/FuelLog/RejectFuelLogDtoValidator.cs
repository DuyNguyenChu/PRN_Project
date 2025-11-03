using api.Dtos.FuelLog;
using FluentValidation;

namespace api.Validator.FuelLog
{
    public class RejectFuelLogDtoValidator : AbstractValidator<RejectFuelLogDto>
    {
        public RejectFuelLogDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");

            RuleFor(x => x.RejectReason)
                .NotNull().NotEmpty().WithMessage("Lí do từ chối là bắt buộc.")
                .MaximumLength(500).WithMessage("Lí do từ chối không quá 500 ký tự."); // SQL là 500
        }
    }
}