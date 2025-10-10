using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models
{
    public interface IEntityBase<TKey>
    {
        public TKey Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
