using System;

namespace api.Dtos.ViolationType
{
    public class CreateViolationTypeDto
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
    }
}
