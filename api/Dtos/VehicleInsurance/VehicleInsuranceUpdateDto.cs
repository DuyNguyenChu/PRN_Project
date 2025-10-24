using System;

namespace api.Dtos.VehicleInsurance
{
    public class VehicleInsuranceUpdateDto
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal? Premium { get; set; }
        public int? Status { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? PolicyNumber { get; set; }
    }
}