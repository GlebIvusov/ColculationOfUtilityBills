using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColculationOfUtilityBills.Models
{
    public class ServicesList
    {
        public int Id { get; set; }
        public int IdService { get; set; }
        public int IdPersonPeriod { get; set; }

        public Service Service { get; set; } = null!;
        public PersonPeriod PersonPeriod { get; set; } = null!;
    }
}
