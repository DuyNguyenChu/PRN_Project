using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Vehicle;
using FluentValidation;

namespace api.Validator.Vehicle
{
    public class VehicleCreateDtoValidator : AbstractValidator<VehicleCreateDto>
    {
        public VehicleCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên xe không được để trống.")
                .MaximumLength(255).WithMessage("Tên xe không được vượt quá 255 ký tự.");

            RuleFor(x => x.VehicleBranchId)
                .NotEmpty().WithMessage("VehicleBranchId không được để trống.")
                .GreaterThan(0).WithMessage("Phải chọn chi nhánh xe.");

            RuleFor(x => x.VehicleModelId)
                .NotEmpty().WithMessage("VehicleModelId không được để trống.")
                .GreaterThan(0).WithMessage("Phải chọn model xe.");

            RuleFor(x => x.VehicleStatusId)
                .NotEmpty().WithMessage("VehicleStatusId không được để trống.")
                .GreaterThan(0).WithMessage("Phải chọn trạng thái xe.");

            RuleFor(x => x.VehicleTypeId)
                .NotEmpty().WithMessage("VehicleStatusId không được để trống.")
                .GreaterThan(0).WithMessage("Phải chọn loại xe.");
        }

    }
}