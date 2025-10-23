using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleRegistration;
using FluentValidation;

namespace api.Validator.Vehicle
{
    public class VehicleRegistrationCreateDtoValidator : AbstractValidator<VehicleRegistrationCreateDto>
    {
        public VehicleRegistrationCreateDtoValidator()
        {
            RuleFor(x => x.VehicleId)
                            .NotEmpty().WithMessage("VehicleId không được để trống.")
                            .GreaterThan(0).WithMessage("Phải chọn xe.");
        }
    }
}