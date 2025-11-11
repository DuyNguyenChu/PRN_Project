using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequestStatus
{
    public class TripRequestStatusDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = "";
        public string Color { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
