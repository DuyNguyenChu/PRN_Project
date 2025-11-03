using api.Dtos.FuelLog;
using FluentValidation;
using api.Helpers;

namespace api.Validator.FuelLog
{
    public class UpdateFuelLogDtoValidator : AbstractValidator<UpdateFuelLogDto>
    {
        public UpdateFuelLogDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID không hợp lệ.");

            // Kế thừa các rule từ Create
            RuleFor(x => x.FuelType)
                .NotNull().NotEmpty().WithMessage("Loại nhiên liệu là bắt buộc.")
                .MaximumLength(50).WithMessage("Loại nhiên liệu quá dài.")
                .Must(fuelType => CommonConstants.FuelType.ContainsKey(fuelType))
                .WithMessage("Loại nhiên liệu không hợp lệ.");

            RuleFor(x => x.Odometer)
                .GreaterThan(0).WithMessage("Số Odometer phải lớn hơn 0.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).When(x => x.UnitPrice.HasValue)
                .WithMessage("Giá đơn vị phải lớn hơn 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");

            RuleFor(x => x.TotalCost)
                .NotNull().NotEmpty().WithMessage("Tổng chi phí là bắt buộc.")
                .GreaterThan(0).WithMessage("Tổng chi phí phải lớn hơn 0.");

            RuleFor(x => x.GasStation)
                .NotNull().NotEmpty().WithMessage("Địa điểm là bắt buộc.")
                .MaximumLength(500).WithMessage("Địa điểm không quá 500 ký tự.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Ghi chú không quá 500 ký tự.");
        }
    }
}