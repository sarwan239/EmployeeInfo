using System;
using System.Collections.Generic;

namespace EmployeeManagement.employee_hub
{
    public partial class EmployeeInfo
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public int? DepartmentId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
