using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.DriverStatus
{
    public class UpdateDriverStatusDto : CreateDriverStatusDto
    {
        public int Id { get; set; } = 0;
        public int? UpdatedBy { get; set; }
    }
}
