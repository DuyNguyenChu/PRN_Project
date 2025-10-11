using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Menu
{
    public class UpdateMenuDto : CreateMenuDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
