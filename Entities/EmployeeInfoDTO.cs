using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Entities
{
    public class EmployeeInfoDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
        public int EmployeeId { get; set; }
    }
}
