using Microsoft.Extensions.Caching.Memory;
using Xero.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using Xero.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Model.PayrollAu;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Xero.Services
{
    public class XeroService : IXeroService
    {

        public const string ConnectionsUrl = "https://api.xero.com/connections";

        public const string BASE_V1 = "https://api.xero.com/api.xro/1.0";
        public const string BASE_V2 = "https://api.xero.com/api.xro/2.0";

        public const string REDIRECT_URI = "https://localhost:44393/api/xero/callback";

        private static string STATE = "123456789";

        public const string AuthorisationUrl = "https://login.xero.com/identity/connect/authorize";

        public static DateTime CurrentDateTime = DateTime.Now;

        private string id_token, access_token, refresh_token;

        private string xeroTenantId = "44622735-69af-4af9-af15-bb1534949f92";

        private IMemoryCache _cache;
        private readonly IConfiguration Configuration;
        public XeroService(
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _cache = cache;
            Configuration = configuration;

            _cache.TryGetValue("id_token", out id_token);
            _cache.TryGetValue("access_token", out access_token);
            _cache.TryGetValue("refresh_token", out refresh_token);
            //_cache.TryGetValue("tenant", out xeroTenantId);
        }

        public async Task<List<Tenant>> GetTenants()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = await client.GetAsync(ConnectionsUrl);
            var content = await response.Content.ReadAsStringAsync();

            //fill the dropdown box based on the results 
            var tenants = JsonConvert.DeserializeObject<List<Tenant>>(content);

            return tenants;
        }

        public async Task<PayItems> GetPayItems()
        {
            var where = "Status=='ACTIVE'";
            var order = "EmailAddress%20DESC";
            var page = 56;

            var apiInstance = new PayrollAuApi();

            var result = await apiInstance.GetPayItemsAsync(access_token, xeroTenantId, null, where, order, page);

            return result;
        }

        public async Task<Employees> GetEmployees()
        {
            try
            {
                var where = "Status==\"ACTIVE\"";
                var order = "LastName ASC";

                var apiInstance = new PayrollAuApi();
                var result = await apiInstance.GetEmployeesAsync(access_token, xeroTenantId, null, where, order);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Employees> UpdateEmployeePayrollCalendar()
        {
            try
            {
                var where = "Status==\"ACTIVE\"";
                var order = "LastName ASC";

                var payRollCalendars = await GetPayrollCalendars();
                var payRollCalendarFortnightId = payRollCalendars._PayrollCalendars.Where(x => x.CalendarType == CalendarType.FORTNIGHTLY).Select(x => x.PayrollCalendarID).FirstOrDefault();

                var employee = await GetEmployee("1646392a-3e28-40b8-8229-ef482068d471");
                var employeeDetails = employee._Employees.FirstOrDefault();
                employeeDetails.PayrollCalendarID = payRollCalendarFortnightId;

                List<Employee> employeeList = new List<Employee>();
                employeeList.Add(employeeDetails);

                var apiInstance = new PayrollAuApi();
                var result = await apiInstance.UpdateEmployeeAsync(access_token, xeroTenantId, (Guid)employeeDetails.EmployeeID, employeeList);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Employees> GetEmployee(string employeeID)
        {
            var apiInstance = new PayrollAuApi();
            var _employeeID = Guid.Parse(employeeID);

            var result = await apiInstance.GetEmployeeAsync(access_token, xeroTenantId, _employeeID);
            return result;
        }

        public async Task<TimesheetObject> GetTimesheet(string employeeID)
        {
            var apiInstance = new PayrollAuApi();
            var timesheetID = Guid.Parse(employeeID);

            var result = await apiInstance.GetTimesheetAsync(access_token, xeroTenantId, timesheetID);
            return result;
        }

        public async Task<Timesheets> GetTimesheets()
        {
            var where = "Status==\"ACTIVE\"";
            var ifModifiedSince = DateTime.Parse("2020-02-06T12:17:43.202-08:00");

            var page = 56;

            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.GetTimesheetsAsync(access_token, xeroTenantId, null, null, null, null);

            return result;
        }

        public async Task<Timesheets> PostTimesheet(List<Timesheet> timesheet)
        {
            var tenantId = await GetApplicationTenant();

            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.CreateTimesheetAsync(access_token, tenantId, timesheet);
            return result;
        }

        public async Task<PayrollCalendars> PostPayrollCalendar(List<PayrollCalendar> payrollCalendar)
        {
            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.CreatePayrollCalendarAsync(access_token, xeroTenantId, payrollCalendar);
            return result;
        }

        public async Task<PayrollCalendars> GetPayrollCalendars()
        {
            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.GetPayrollCalendarsAsync(access_token, xeroTenantId, null, null, null);
            return result;
        }

        public async Task<PayRuns> PostPayRun(List<PayRun> payRuns)
        {
            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.CreatePayRunAsync(access_token, xeroTenantId, payRuns);
            return result;
        }

        public async Task<PayRuns> GetPayruns()
        {
            var apiInstance = new PayrollAuApi();
            var result = await apiInstance.GetPayRunsAsync(access_token, xeroTenantId, null, null, null);
            return result;
        }

        public async Task<List<EmployeeXeroDto>> GetEmployeeDetails(StaffDto staffs)
        {
            List<EmployeeXeroDto> employeeList = new List<EmployeeXeroDto>();

            var apiInstance = new PayrollAuApi();
            var employeesApi = await apiInstance.GetEmployeesAsync(access_token, xeroTenantId, null, null, null);

            foreach (var employee in employeesApi._Employees)
            {
                var staffCount = staffs.Staffs.Where(x => x.LastName.ToLower() == employee.LastName.ToLower()
                                                    && x.FirstName.ToLower() == employee.FirstName.ToLower());
                if (staffCount.Count() > 0)
                {
                    var result = await apiInstance.GetEmployeeAsync(access_token, xeroTenantId, (Guid)employee.EmployeeID);
                    var employeeDetails = result._Employees[0];

                    employeeList.Add(new EmployeeXeroDto()
                    {
                        FirstName = employeeDetails.FirstName,
                        LastName = employeeDetails.LastName,
                        MiddleNames = employeeDetails.MiddleNames,

                        EmployeeId = employeeDetails.EmployeeID,
                        OrdinaryEarningsRateID = employeeDetails.OrdinaryEarningsRateID,
                        PayrollCalendarID = employeeDetails.PayrollCalendarID,

                        DateOfBirth = employeeDetails.DateOfBirth,
                        TaxFileNumber = employeeDetails.TaxDeclaration.TaxFileNumber,
                        Id = staffCount.Select(x => x.Id).FirstOrDefault()
                    });
                }
            }

            return employeeList;
        }


        public async Task<string> GetApplicationTenant()
        {
            var tenants = await GetTenants();
            var tenantId = tenants.Where(x => x.Name == Configuration["XeroConfiguration:ApplicationName"].ToString()).Select(x => x.Id).FirstOrDefault();
            return tenantId;
        }



    }
}
