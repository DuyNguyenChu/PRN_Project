using System;

namespace api.Dtos.ViolationType
{
    public class ViolationTypeDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Color { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
