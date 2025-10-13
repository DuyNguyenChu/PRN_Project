using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Helpers;

namespace api.Extensions
{
    public class SearchQuery
    {
        [Range(1, Int32.MaxValue)]
        public int PageIndex { get; set; } = 1;
        [Range(1, 200)]
        public int PageSize { get; set; } = 10;
        [MaxLength(500)]
        public string Keyword { get; set; } = string.Empty;
        [MaxLength(4), SortTypeValidate]
        public string SortType { get; set; } = "asc";
        public string OrderBy { get; set; } = string.Empty;

    }
}
