namespace api.Dtos.MaintenanceRecord
{
    public class CreateMaintenanceRecordDetailDto
    {
        public string Description { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}