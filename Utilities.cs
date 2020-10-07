using EmployeeManagement.employee_hub;
using EmployeeManagement.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public static class Utilities
    {
        public static IConfiguration configuration { get; set; }

        /// <summary>
        /// Function to send Welcome email notification to the cocerned employee
        /// </summary>
        /// <param name="employeeInfo"></param>
        /// <returns></returns>
        public static bool SendWelcomeMail(EmployeeInfoDTO employeeInfo)
        {
            return true;
        }


        /// <summary>
        /// Function to send delete email notification to the concerned employee
        /// </summary>
        /// <param name="employeeInfo"></param>
        /// <returns></returns>
        public static bool SendDeleteNotification(EmployeeInfoDTO employeeInfo)
        {
            return true;
        }

        /// <summary>
        /// Validates an input email id and returns true if valid and false in case of invalid email id.
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns>boolean value</returns>
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress omailAddress = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to write logs
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            var logfile = configuration["LogFileName"];
            if (!string.IsNullOrEmpty(logfile))
            {
                var processId = Process.GetCurrentProcess().Id;
                logfile = logfile.Replace(".txt", "_" + processId + "_" + DateTime.Now.ToString("yyyyMMddHH") + ".txt");
                TextWriter logWriter = new StreamWriter(logfile, true);
                logWriter.WriteLine("{0} : {1}", DateTime.Now, message);
                logWriter.Flush();
                logWriter.Close();
            }
        }

    }
}
