namespace api.Dtos.FuelLog
{
    public class CreateFuelLogDto
    {
        public int VehicleId { get; set; }
        public int? TripId { get; set; }
        public int Odometer { get; set; }
        public string FuelType { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public decimal Quantity { get; set; } // Đã thêm
        public decimal TotalCost { get; set; }
        public string GasStation { get; set; } = null!; // Đã đổi
        public string Notes { get; set; } = string.Empty; // SQL yêu cầu NOT NULL
    }
}