using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Driver
{
    public class UpdateDriverDto
    {
        public int Id { get; set; }
        public decimal BaseSalary { get; set; }
        public int DriverStatusId { get; set; }
        public int UpdatedBy { get; set; }
    }

}
