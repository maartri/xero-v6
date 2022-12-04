using Microsoft.AspNetCore.Mvc;
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

using Xero.Helper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using Xero.Core.Services.Interfaces;

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
        private readonly ITokenService _tokenS;
        private readonly IConfiguration Configuration;

        public XeroController(
                IMemoryCache cache,
                IXeroService xeroS,
                IConfiguration configuration,
                ITokenService tokenS
            )
        {
            _cache = cache;
            _xeroS = xeroS;
            _tokenS = tokenS;

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

        [HttpGet("generate-login-token")]
        public async Task<IActionResult> GetGenerateLoginToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("e-FU7uLzfe%qAGq9-e^qT6+yqkbN_$uuWuQ9xv"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                   issuer: "https://localhost:44393/",
                   audience: "https://localhost:44393/",
                   claims: new List<Claim>(),
                   expires: DateTime.Now.AddDays(5000),
                   signingCredentials: signinCredentials
               );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(tokenString);
        }

        [HttpGet("refresh-tokens")]
        //[Authorize]
        public async Task<IActionResult> GetRefreshTokens()
        {
            return Ok(await _tokenS.GetRefreshTokens());           
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

            var token = new Tokens()
            {
                IdToken = id_token,
                AccessToken = access_token,
                RefreshToken = refresh_token
            };

            var serializeJSON = Newtonsoft.Json.JsonConvert.SerializeObject(token);


            var logPath = Environment.CurrentDirectory;
            var OutputFilePath = Path.Combine(logPath, "Output", "token.json");

            using (var writer = System.IO.File.CreateText(OutputFilePath))
            {
                writer.WriteLine(serializeJSON); 
            }

            return Ok(token);
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
        [HttpPost("timesheet/{dbname}")]
        public async Task<IActionResult> PostTimesheet([FromBody] Timesheet timesheet, string dbname)
        {
            List<Timesheet> timesheets = new List<Timesheet>() {
                timesheet
            };

            return Ok(await _xeroS.PostTimesheet(timesheets));
        }

        [HttpPost("timesheets")]
        public async Task<IActionResult> PostTimesheets([FromBody] TimesheetsDto timesheet)
        {
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



}
