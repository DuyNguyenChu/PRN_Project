namespace api.Dtos.FuelLog
{
    public class RejectFuelLogDto
    {
        public int Id { get; set; }
        public string RejectReason { get; set; } = null!;
    }
}