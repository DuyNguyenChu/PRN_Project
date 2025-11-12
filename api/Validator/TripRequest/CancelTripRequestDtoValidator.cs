using api.Dtos.TripRequest;
using api.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Validator.TripRequest
{
    public class CancelTripRequestDtoValidator : AbstractValidator<CancelTripRequestDto>
    {
        public CancelTripRequestDtoValidator()
        {
            
        }
    }
}
