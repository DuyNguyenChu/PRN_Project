using System;

namespace api.Dtos.ViolationType
{
    public class UpdateViolationTypeDto : CreateViolationTypeDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
