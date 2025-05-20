using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColculationOfUtilityBills.Models
{
    public class UserInputData
    {
        public int IdService { get; set; }
        public string NameService { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
        public int Value { get; set; }

    }
}
