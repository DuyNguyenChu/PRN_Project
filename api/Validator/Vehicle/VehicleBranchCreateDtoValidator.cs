using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleBranch;
using FluentValidation;

namespace api.Validator.Vehicle
{
    public class VehicleBranchCreateDtoValidator : AbstractValidator<VehicleBranchCreateDto>
    {
        public VehicleBranchCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên chi nhánh không được để trống.")
                .MaximumLength(100).WithMessage("Tên chi nhánh không được vượt quá 100 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
        }
    }
}