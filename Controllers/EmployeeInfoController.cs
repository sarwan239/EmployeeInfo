using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.employee_hub;
using Microsoft.Web.Administration;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using EmployeeManagement.Entities;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Controllers
{

    public class EmployeeInfoController : Controller
    {
        private readonly employee_hubContext _context;

        public static IConfiguration configuration { get; set; }

        public EmployeeInfoController(employee_hubContext context)
        {
            _context = context;
            var builder = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            configuration = builder.Build();
        }



        /// <summary>
        /// Adds an employee to the database after validating FirstName,LastName,EmailId,DepartmentName and sends the Welcome email
        /// </summary>
        /// <param name="employeeInfoDTO">Employee class object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeInfo/AddEmployee")]
        public ActionResult AddEmployee([FromBody] EmployeeInfoDTO employeeInfoDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeInfoDTO.FirstName) && !string.IsNullOrEmpty(employeeInfoDTO.LastName) &&
                    !string.IsNullOrEmpty(employeeInfoDTO.EmailId) && !string.IsNullOrEmpty(employeeInfoDTO.DepartmentName))
                {
                    if (Utilities.IsValidEmail(employeeInfoDTO.EmailId))
                    {
                        EmployeeInfo employee = new EmployeeInfo();
                        employee.FirstName = employeeInfoDTO.FirstName;
                        employee.LastName = employeeInfoDTO.LastName;
                        employee.EmailId = employeeInfoDTO.EmailId;
                        employee.DepartmentId = _context.DepartmentInfo.FirstOrDefault(x => x.DepartmentName == employeeInfoDTO.DepartmentName).DepartmentId;

                        if (string.IsNullOrEmpty(employeeInfoDTO.ManagerName))
                        {
                            bool isHeadPresent = _context.EmployeeInfo.Select(x => x.ManagerId == null).Any();
                            if (isHeadPresent)
                            {
                                return Content("Manager name cannot be empty");
                            }
                        }
                        else
                        {
                            employee.ManagerId = _context.EmployeeInfo.FirstOrDefault(x => x.FirstName == employeeInfoDTO.ManagerName).EmployeeId;
                        }
                        _context.EmployeeInfo.Add(employee);
                        _context.SaveChanges();
                        employeeInfoDTO.EmployeeId = employee.EmployeeId;
                        Utilities.WriteLog($"Employee {employee.EmployeeId} added.");
                        Utilities.SendWelcomeMail(employeeInfoDTO);

                    }
                    else
                    {
                        return Content($"{employeeInfoDTO.EmailId} is an invalid email address");
                    }
                }
                else
                {
                    return Content("** FirstName,LastName,EmailId,DepartmentName cannot be empty");
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : AddEmployee() : " + ex.ToString());
            }

            return Content("Success");
        }

        /// <summary>
        /// Updates the employee details based on the given employee id
        /// </summary>
        /// <param name="employeeInfoDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeInfo/UpdateEmployee")]
        public ActionResult UpdateEmployee([FromBody] EmployeeInfoDTO employeeInfoDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeInfoDTO.EmployeeId.ToString()))
                {
                    var employee = _context.EmployeeInfo.FirstOrDefault(x => x.EmployeeId == employeeInfoDTO.EmployeeId);
                    if (!string.IsNullOrEmpty(employeeInfoDTO.FirstName) && !string.IsNullOrEmpty(employeeInfoDTO.LastName) &&
                        !string.IsNullOrEmpty(employeeInfoDTO.EmailId) && !string.IsNullOrEmpty(employeeInfoDTO.DepartmentName))
                    {
                        if (Utilities.IsValidEmail(employeeInfoDTO.EmailId))
                        {
                            employee.FirstName = employeeInfoDTO.FirstName;
                            employee.LastName = employeeInfoDTO.LastName;
                            employee.EmailId = employeeInfoDTO.EmailId;
                            employee.DepartmentId = _context.DepartmentInfo.FirstOrDefault(x => x.DepartmentName == employeeInfoDTO.DepartmentName).DepartmentId;
                            employee.ManagerId = _context.EmployeeInfo.FirstOrDefault(x => x.FirstName == employeeInfoDTO.ManagerName).EmployeeId;
                            _context.SaveChanges();
                            Utilities.WriteLog($"Employee {employee.EmployeeId} updated.");
                        }
                        else
                        {
                            return Content($"{employeeInfoDTO.EmailId} is an invalid email address");
                        }
                    }
                    else
                    {
                        return Content("** FirstName,LastName,EmailId,DepartmentName cannot be empty");
                    }
                }
                else
                {
                    return Content("** EmployeeId cannot be empty");
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : UpdateEmployee() : " + ex.ToString());
            }

            return Content("Success");
        }


        /// <summary>
        /// Deletes an employee from the database based on the given employee id and sends the deleted notification
        /// </summary>
        /// <param name="employeeInfoDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EmployeeInfo/DeleteEmployee")]
        public ActionResult DeleteEmployee([FromBody] EmployeeInfoDTO employeeInfoDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeInfoDTO.EmployeeId.ToString()))
                {
                    var employee = _context.EmployeeInfo.FirstOrDefault(x => x.EmployeeId == employeeInfoDTO.EmployeeId);
                    bool isManager = _context.EmployeeInfo.Select(x => x.ManagerId == employee.EmployeeId).Any();
                    _context.Remove(employee);
                    Utilities.WriteLog($"Employee {employee.EmployeeId} was deleted.");
                    Utilities.SendDeleteNotification(employeeInfoDTO);
                    if (isManager)
                    {
                        foreach (EmployeeInfo oEmployee in _context.EmployeeInfo)
                        {
                            oEmployee.ManagerId = _context.EmployeeInfo.FirstOrDefault(x => x.FirstName == employeeInfoDTO.ManagerName).EmployeeId;
                        }
                    }
                    _context.SaveChanges();
                }
                else
                {
                    return Content("** EmployeeId cannot be empty");
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : DeleteEmployee() : " + ex.ToString());
            }

            return Content("Success");
        }

        /// <summary>
        /// Returns the employee role based on the employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/EmployeeInfo/GetEmployeeStatus")]
        public string GetEmployeeStatus(int employeeId)
        {
            try
            {
                if (!string.IsNullOrEmpty(employeeId.ToString()))
                {
                    var employee = _context.EmployeeInfo.FirstOrDefault(x => x.EmployeeId == employeeId);
                    if (employee != null)
                    {
                        if (employee.ManagerId == null && _context.EmployeeInfo.Select(x => x.ManagerId == employeeId).Any())
                        {
                            return "Head";
                        }
                        else if (_context.EmployeeInfo.Select(x => x.ManagerId == employeeId).Any())
                        {
                            return "Manager";
                        }
                        else
                        {
                            return "Associate";
                        }
                    }
                    else
                    {
                        return "Unable to find employee with given EmployeeId";
                    }
                }
                else
                {
                    return "** EmployeeId cannot be empty";
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : GetEmployeeStatus() : " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Returns the list of all employees present in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/EmployeeInfo/GetAllEmployees")]
        public ActionResult<IEnumerable<EmployeeInfo>> GetAllEmployees()
        {
            try
            {
                return _context.EmployeeInfo.OrderBy(x => x.FirstName).ToList();
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : GetAllEmployees() : " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Returns the list of all employees who meets the provided filter criteria.
        /// </summary>
        /// <param name="employeeInfoDTO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/EmployeeInfo/GetEmployeesWithFilter")]
        public ActionResult<IEnumerable<EmployeeInfo>> GetEmployeesWithFilter([FromBody] EmployeeInfoDTO employeeInfoDTO)
        {
            try
            {
                return _context.EmployeeInfo.Where(
                                x => x.EmployeeId == employeeInfoDTO.EmployeeId &&
                                x.DepartmentId == _context.DepartmentInfo.FirstOrDefault(x => x.DepartmentName == employeeInfoDTO.DepartmentName).DepartmentId &&
                                x.FirstName == employeeInfoDTO.FirstName &&
                                x.LastName == employeeInfoDTO.LastName).
                    OrderBy(x => x.FirstName).ToList();
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("Exception : GetEmployeesWithFilter() : " + ex.ToString());
                return null;
            }
        }

    }
}
