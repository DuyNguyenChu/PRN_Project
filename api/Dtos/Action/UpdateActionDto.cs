using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Action
{
    public class UpdateActionDto : CreateActionDto
    {
        public int Id { get; set; }
        public int UpdatedBy { get; set; }

    }
}
