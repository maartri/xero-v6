using System.Collections.Generic;
using System.Threading.Tasks;
using Xero.Interfaces;
using Xero.Models;
using Xero.NetStandard.OAuth2.Model.PayrollAu;

namespace Xero.Interfaces
{
    public interface IXeroService
    {
        Task<List<Tenant>> GetTenants();
        Task<PayItems> GetPayItems();
        Task<Employees> GetEmployees();
        Task<Employees> GetEmployee(string employeeID);
        Task<TimesheetObject> GetTimesheet(string employeeID);
        Task<Timesheets> GetTimesheets();
        Task<Timesheets> PostTimesheet(List<Timesheet> timesheet);
        Task<PayrollCalendars> PostPayrollCalendar(List<PayrollCalendar> payrollCalendar);
        Task<PayrollCalendars> GetPayrollCalendars();
        Task<PayRuns> PostPayRun(List<PayRun> payRuns);
        Task<PayRuns> GetPayruns();
        Task<Employees> UpdateEmployeePayrollCalendar();





        Task<List<EmployeeXeroDto>> GetEmployeeDetails(StaffDto staffs);

    }
}
