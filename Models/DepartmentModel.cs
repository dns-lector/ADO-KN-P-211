using ADO_KN_P_211.EfContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_KN_P_211.Models
{
    public class DepartmentModel
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string? InternationalName { get; set; }

        public static DepartmentModel FromEntity(Department entity) =>
            new()   // Mapping - перетворення моделей
            { 
                Id = entity.Id,
                Name = entity.Name,
                InternationalName = entity.InternationalName,
            };
    }
}
