using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Menu
{
    public class MenuPermissionDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string TreeIds { get; set; } = null!;
        public string Name { get; set; } = null!;
        //public int SortOrder { get; set; }
        public bool HasCreate { get; set; }
        public bool HasRead { get; set; }
        public bool HasUpdate { get; set; }
        public bool HasDelete { get; set; }
        public bool HasExport { get; set; }
        public bool HasApprove { get; set; }

    }
}
