using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class PermissionAggregate
    {
        public int MenuId { get; set; }
        public List<int> ActionIds { get; set; } = new List<int>();

    }
}
