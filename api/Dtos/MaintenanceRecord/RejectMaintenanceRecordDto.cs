namespace api.Dtos.MaintenanceRecord
{
    public class RejectMaintenanceRecordDto
    {
        public int Id { get; set; }
        public string RejectReason { get; set; } = null!;
    }
}