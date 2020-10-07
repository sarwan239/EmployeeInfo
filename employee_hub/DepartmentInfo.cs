using System;
using System.Collections.Generic;

namespace EmployeeManagement.employee_hub
{
    public partial class DepartmentInfo
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
