using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Xero.Core.Data.Entities;
using Xero.Core.Services.Interfaces;
using Xero.Interfaces;

using Xero.Core.Services.Interfaces;
using System.Xml.Linq;

namespace Xero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdamasController : Controller
    {
        private IStaffService staffService;
        private IXeroService xeroService;
        private readonly ITokenService _tokenS;
        private readonly IConfiguration config;

        public AdamasController(IStaffService _staffService, IXeroService _xeroService, ITokenService tokenS, IConfiguration _config) { 
            staffService = _staffService;
            xeroService = _xeroService;
            _tokenS = tokenS;
            config = _config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get Xero EarningsRates
            var payItems = await xeroService.GetPayItems();

            if(payItems._PayItems.EarningsRates.Count == 0)
            {
                return BadRequest("No rates available");
            }

            // Get All ItemTypes
            //var items = await staffService.GetItemTypes(new ItemTypes() { 
            //    RosterGroup = "ONEONONE"
            //});

            //var commonPayItems = payItems._PayItems.EarningsRates.Where(x => items.Select(x => x.Title).Contains(x.Name));


            return Ok(await staffService.GetStaffs());
        }
        [HttpGet("GetEarningsRate")]
        public async Task<IActionResult> GetEarningsRate()
        {
            var payItems = await xeroService.GetPayItems();
            return Ok(payItems);
        }


        [HttpGet("PostEarningsRate/{dbName}")]
        public async Task<IActionResult> PostEarningsRate(string dbName)
        {
            var strDbConn = config.GetValue<string>("ConnectionStrings:Production");
            await staffService.ChangeDbConnection(dbName, strDbConn);

            // Get RefreshTokens
            var token = await _tokenS.GetRefreshTokens();
            // Get Xero EarningsRates
            var payItems = await xeroService.GetPayItems();
            return Ok(await staffService.PostAllEarningsRateIds(payItems._PayItems.EarningsRates));
        }

        [HttpGet("PostXeroEmployeeId/{dbName}")]
        [HttpGet]
        public async Task<IActionResult> PostXeroEmployeeId(string dbName)
        {
            var strDbConn = config.GetValue<string>("ConnectionStrings:Production");
            await staffService.ChangeDbConnection(dbName, strDbConn);

            // Get RefreshTokens
            var token = await _tokenS.GetRefreshTokens();
            // Get Xero EarningsRates
            var employees = await xeroService.GetEmployees();
            return Ok(await staffService.PostXeroEmployeeId(employees));
        }

        [HttpGet]
        public async Task<IActionResult> GetHello(string dbName)
        {
            var strDbConn = config.GetValue<string>("ConnectionStrings:Production");
            await staffService.ChangeDbConnection(dbName, strDbConn);
            return Ok();
        }

    }
}
