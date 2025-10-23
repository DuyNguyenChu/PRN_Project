using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleStatus;
using FluentValidation;

namespace api.Validator.Vehicle
{
    public class VehicleStatusCreateDtoValidator : AbstractValidator<VehicleStatusCreateDto>
    {
        public VehicleStatusCreateDtoValidator()
        {
            // Name không được để trống, tối đa 100 ký tự
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên trạng thái xe không được để trống.")
                .MaximumLength(100).WithMessage("Tên trạng thái xe không được vượt quá 100 ký tự.");

            // Color không được để trống, tối đa 50 ký tự
            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Màu trạng thái không được để trống.")
                .MaximumLength(50).WithMessage("Màu trạng thái không được vượt quá 50 ký tự.");

            // Description có thể để trống nhưng tối đa 500 ký tự nếu có
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

            // CreatedBy nếu có thì phải > 0
            RuleFor(x => x.CreatedBy)
                .GreaterThan(0).WithMessage("Người tạo phải hợp lệ.")
                .When(x => x.CreatedBy.HasValue);
        }
    }
}