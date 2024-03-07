using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_KN_P_211.EfContext
{
    public class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        public DateTime? DeleteDt { get; set; }

        public String? InternationalName { get; set; }

        ///// NAVIGATION PROPERTIES /////////
        public List<Manager> MainWorkers { get; set; }
        public IEnumerable<Manager> SecondaryWorkers { get; set; }
    }
}
