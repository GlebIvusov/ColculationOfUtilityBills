using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColculationOfUtilityBills.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Rate { get; set; }
        public decimal Standart { get; set; }
        public string? MeasureUnit { get; set; }

        public List<ServicesList> ServicesLists { get; set; } = [];

    }
}
