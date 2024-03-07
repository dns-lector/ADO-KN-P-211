using ADO_KN_P_211.EfContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_KN_P_211.Models
{
    public class ManagerModel
    {
        public String Surname { get; set; }
        public String Name { get; set; }
        public String Secname { get; set; }
        public String MainDep { get; set; }
        public String? SecDep { get; set; }
        public String? Chief { get; set; }
        public List<String> Departments { get; set; }

        public ManagerModel(Manager entity)
        {
            Surname = entity.Surname;
            Name = entity.Name;
            Secname = entity.Secname;
        }
    }
}
