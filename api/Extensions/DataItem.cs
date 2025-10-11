using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Extensions
{
    public class DataItem<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

}
