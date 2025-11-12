using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class DriverStatusAggregate
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Color { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
