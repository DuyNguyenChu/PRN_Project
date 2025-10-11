using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Menu
{
    public class MenuDetailDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string TreeIds { get; set; } = null!;
        public string MenuType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? ClassName { get; set; }
        public int SortOrder { get; set; } = 1;
        public bool IsAdminOnly { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<int> ActionIds { get; set; } = new List<int>();

    }
}
