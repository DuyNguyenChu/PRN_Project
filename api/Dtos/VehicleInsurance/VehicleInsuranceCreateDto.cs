using System;

namespace api.Dtos.VehicleInsurance
{
    public class VehicleInsuranceCreateDto
    {
        public int VehicleId { get; set; }
        public string InsuranceProvider { get; set; } = null!;
        public string PolicyNumber { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal Premium { get; set; }
        public int Status { get; set; }
    }
}