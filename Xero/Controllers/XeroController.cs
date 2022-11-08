using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;

using Xero.Models;
using System.Net.Http.Headers;
using Xero.Services;
using Xero.Interfaces;

using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Model.PayrollAu;
using Microsoft.Extensions.Configuration;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Xero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XeroController : Controller
    {
        private static string CLIENT_ID;
        private static string SCOPES;
        private static string CODE_VERIFIER;

        private static string APPLICATION_NAME = string.Empty;
        private static string REDIRECT_URI = string.Empty;

        public const string ConnectionsUrl = "https://api.xero.com/connections";

        private static string STATE = "123456789";

        public const string AuthorisationUrl = "https://login.xero.com/identity/connect/authorize";

        public static DateTime CurrentDateTime = DateTime.Now;


        private IMemoryCache _cache;
        private readonly IXeroService _xeroS;
        private readonly IConfiguration Configuration;

        public XeroController(
                IMemoryCache cache,
                IXeroService xeroS,
                IConfiguration configuration
            )
        {
            _cache = cache;
            _xeroS = xeroS;
            Configuration = configuration;

            CLIENT_ID           = Configuration["XeroConfiguration:ClientId"].ToString();
            SCOPES              = Configuration["XeroConfiguration:Scopes"].ToString();
            CODE_VERIFIER       = Configuration["XeroConfiguration:CodeVerifier"].ToString();
            APPLICATION_NAME    = Configuration["XeroConfiguration:ApplicationName"].ToString();
            REDIRECT_URI        = Configuration["XeroConfiguration:REDIRECT_URI"].ToString();
        }

        // GET: api/<XeroController>
        [HttpGet]
        public IActionResult GetUrl()
        {
            var clientId    = CLIENT_ID;
            var scopes      = Uri.EscapeUriString(SCOPES);
            var redirectUri = REDIRECT_URI;
            var state       = STATE;

            string codeChallenge;
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(CODE_VERIFIER));
                codeChallenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
            var authLink = $"{AuthorisationUrl}?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scopes}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";
            return Redirect(authLink);
        }

        [HttpGet("refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens()
        {
            string refresh_token;
            const string url = "https://identity.xero.com/connect/token";
            var client = new HttpClient();
            _cache.TryGetValue("refresh_token", out refresh_token);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", CLIENT_ID),
                new KeyValuePair<string, string>("refresh_token", refresh_token),
            });

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));

            var response = await client.PostAsync(url, formContent);
            var content = await response.Content.ReadAsStringAsync();
            var tokens = JObject.Parse(content);


            string access_token = tokens["access_token"]?.ToString();
            _cache.Set("access_token", access_token, cacheEntryOptions);

            string refresh_token_result = tokens["refresh_token"]?.ToString();
            _cache.Set("refresh_token", refresh_token_result, cacheEntryOptions);

             return Ok(new Tokens()
            {
                IdToken = null,
                AccessToken = access_token,
                RefreshToken = refresh_token,
                //Tenants = tenants.Where(x => x.Name == APPLICATION_NAME).Select(x => x).ToList()
            });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> GetCallback()
        {
            string code = HttpContext.Request.Query["code"].ToString();
            string scope = HttpContext.Request.Query["scope"].ToString();
            string state = HttpContext.Request.Query["state"].ToString();

            const string url = "https://identity.xero.com/connect/token";

            var client = new HttpClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", CLIENT_ID),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", REDIRECT_URI),
                new KeyValuePair<string, string>("code_verifier", CODE_VERIFIER),
            });

            var response = await client.PostAsync(url, formContent);

            //read the response and populate the boxes for each token
            //could also parse the expiry here if required
            var content = await response.Content.ReadAsStringAsync();
            var tokens = JObject.Parse(content);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));

            string id_token = tokens["id_token"]?.ToString();
            _cache.Set("id_token", id_token, cacheEntryOptions);

            string access_token = tokens["access_token"]?.ToString();
            _cache.Set("access_token", access_token, cacheEntryOptions);


            string refresh_token = tokens["refresh_token"]?.ToString();
            _cache.Set("refresh_token", refresh_token, cacheEntryOptions);

            return Ok(new Tokens()
            {
                IdToken = id_token,
                AccessToken = access_token,
                RefreshToken = refresh_token,
                //Tenants = tenants.Where(x => x.Name == APPLICATION_NAME).Select(x => x).ToList()
            });
        }

        [HttpGet("hello")]
        public ActionResult GoToCallBack()
        {
            return View();
        }

        [HttpGet("tokens")]
        public async Task<IActionResult> GetTokens()
        {
            string id_token, access_token, refresh_token;

            _cache.TryGetValue("id_token", out id_token);
            _cache.TryGetValue("access_token", out access_token);
            _cache.TryGetValue("refresh_token", out refresh_token);

            var tenants = await _xeroS.GetTenants();

            return Ok(new Tokens()
            {
                IdToken = id_token,
                AccessToken = access_token,
                RefreshToken = refresh_token,
                Tenants = tenants
            });
        }

        [HttpGet("tenants")]
        public async Task<IActionResult> GetTenants()
        {
            return Ok(await _xeroS.GetTenants());
        }

        //api/xero/payitems
        [HttpGet("payitems")]
        public async Task<IActionResult> GetPayItems()
        {
            return Ok(await _xeroS.GetPayItems());
        }

        //api/xero/employees
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            return Ok(await _xeroS.GetEmployees());
        }

        //api/employees-payrollcalendar
        [HttpPut("employees-payrollcalendar")]
        public async Task<IActionResult> UpdateEmployeePayrollCalendar()
        {
            return Ok(await _xeroS.UpdateEmployeePayrollCalendar());
        }

        //api/xero/employee
        [HttpGet("employee/{employeeID}")]
        public async Task<IActionResult> GetEmployees(string employeeID)
        {
            return Ok(await _xeroS.GetEmployee(employeeID));
        }

        //api/xero/timesheet
        [HttpGet("timesheet/{employeeID}")]
        public async Task<IActionResult> GetTimesheet(string employeeID)
        {
            return Ok(await _xeroS.GetTimesheet(employeeID));
        }

        //api/xero/timesheet
        [HttpGet("timesheets")]
        public async Task<IActionResult> GetTimesheets()
        {
            return Ok(await _xeroS.GetTimesheets());
        }


        //  api/xero/timesheet
        //  Add Payroll Calendar
        [HttpPost("timesheet")]
        public async Task<IActionResult> PostTimesheet([FromBody] Timesheet timesheet)
        {
            //List<Timesheet> timesheet = new List<Timesheet>() {
            //    new Timesheet(){
            //        EmployeeID = Guid.Parse("80062955-9bc6-4c8e-8cb2-e86148d9923f"),
            //        StartDate = DateTime.Now.AddMonths(-2),
            //        EndDate = DateTime.Now.AddMonths(12),
            //        Status = TimesheetStatus.DRAFT,
            //        TimesheetLines = new List<TimesheetLine>() {
            //            new TimesheetLine(){
            //               EarningsRateID = Guid.Parse("146125ff-9611-4086-9b7c-fbe73b8ff706"),
            //               NumberOfUnits = new List<double>(){ 0,8,8,8,8,8,0 }
            //            }
            //        }
            //    }
            //};

            List<Timesheet> timesheets = new List<Timesheet>() {
                timesheet
            };

            return Ok(await _xeroS.PostTimesheet(timesheets));
        }

        [HttpPost("timesheets")]
        public async Task<IActionResult> PostTimesheets([FromBody] TimesheetsDto timesheet)
        {
            //var employee = await _xeroS.GetEmployees();

            //List<Timesheet> timesheet = new List<Timesheet>() {
            //    new Timesheet(){
            //        EmployeeID = employee._Employees.OrderBy(x => x.UpdatedDateUTC).Last().EmployeeID,
            //        StartDate = new DateTime(2022,9,1),
            //        EndDate = new DateTime(2022,9,14),
            //        Status = TimesheetStatus.DRAFT,
            //        TimesheetLines = new List<TimesheetLine>() {
            //            new TimesheetLine(){
            //               EarningsRateID = Guid.Parse("a7f3c2ec-fdd3-496c-bc14-1ad7e742f96a"),
            //               NumberOfUnits = new List<double>(){ 0,8,8,8,8,8,0, 0, 8, 8, 8, 8, 8, 0 }
            //            }
            //        }
            //    }
            //};
            //return Ok(true);
            return Ok(await _xeroS.PostTimesheet(timesheet.Timesheets));
        }

        //api/xero/payroll-calendar
        [HttpPost("payroll-calendar")]
        public async Task<IActionResult> PostPayrollCalendar()
        {
            List<PayrollCalendar> payrollCalendar = new List<PayrollCalendar>() {
                new PayrollCalendar(){
                  Name = "Fortnightly",
                  CalendarType = CalendarType.FORTNIGHTLY,
                  StartDate = new DateTime(2022, 9, 1),
                  PaymentDate = new  DateTime(2022, 9, 15),
                }
            };

            return Ok(await _xeroS.PostPayrollCalendar(payrollCalendar));
        }

        //api/xero/payroll-calendar
        [HttpGet("payroll-calendar")]
        public async Task<IActionResult> GetPayrollCalendars()
        {
            return Ok(await _xeroS.GetPayrollCalendars());
        }

        //api/xero/payrun
        [HttpPost("payrun")]
        public async Task<IActionResult> PostPayRun()
        {
            List<PayRun> payRuns = new List<PayRun>();

            PayRun payRun = new PayRun();
            var payRollCalendars = await _xeroS.GetPayrollCalendars();
            var payRollCalendarFortnightId = payRollCalendars._PayrollCalendars.Where(x => x.CalendarType == CalendarType.FORTNIGHTLY).Select(x => x.PayrollCalendarID).FirstOrDefault();
            payRun.PayrollCalendarID = payRollCalendarFortnightId;

            payRuns.Add(payRun);

            return Ok(await _xeroS.PostPayRun(payRuns));
        }

        //api/xero/payruns
        [HttpGet("payruns")]
        public async Task<IActionResult> GetPayruns()
        {
            return Ok(await _xeroS.GetPayruns());
        }

        ////api/xero/payruns
        //[HttpGet("payroll-calendar")]
        //public async Task<IActionResult> GetPayrollCalendars()
        //{
        //    return Ok(await _xeroS.PayrollCalendars());
        //}

        [HttpPost("employee-details")]
        public async Task<IActionResult> GetEmployeeDetails([FromBody] StaffDto staffs)
        {
            return Ok(await _xeroS.GetEmployeeDetails(staffs));
        }



    }


    public class Tokens
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public List<Tenant> Tenants { get; set; }
    }
}
