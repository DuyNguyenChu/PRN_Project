using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Extensions
{
    public class DetailStatusDto<T> : DataItem<T>
    {
        public string Color { get; set; } = null!;
    }

}
