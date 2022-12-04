using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xero.Core.Services.Interfaces;
using Xero.Core.Services.Context;
using Xero.Core.Data.Entities;
using Xero.Core.Data.Dto;
using Microsoft.EntityFrameworkCore;

using Xero.NetStandard.OAuth2.Model.PayrollAu;
using Xero.Core.Services.Helper;
using Microsoft.Data.SqlClient;

namespace Xero.Core.Services.Main
{
    public class StaffService : IStaffService
    {
        private readonly DatabaseContext context;
        public StaffService(DatabaseContext _context) {
            context = _context;
        }


        public async Task<List<Staff>> GetStaffs()
        {

            var staffs = await (from s in context.Staff select new Staff() {
                UniqueID = s.UniqueID,
                AccountNo = s.AccountNo
            }).ToListAsync();
            return staffs;

        }

        public async Task<List<ItemTypesDto>> GetItemTypes(ItemTypes it){
            var items = await (from item in context.ItemTypes
                               where item.RosterGroup == it.RosterGroup
                               select new ItemTypesDto()
                               {
                                   Recnum = item.Recnum,
                                   Title = item.Title,
                                   RosterGroup = it.RosterGroup,
                                   Amount = it.Amount,
                                   Unit = it.Unit
                               }).ToListAsync();

            return items;
        }

        public async Task<bool> PostXeroEmployeeId(Employees employees)
        {
            try
            {
                var _staffs = (from s in context.Staff
                              where s.FirstName != null && s.LastName != null
                              select new { s.SQLID, s.FirstName, s.LastName }).ToList();

                var _xeroEmp = (from e in employees._Employees
                               select new { e.EmployeeID, e.FirstName, e.LastName }).ToList();

                var intersectEmployees = (from sf in _staffs
                           join x in _xeroEmp
                           on new { FirstName = sf.FirstName, LastName = sf.LastName } equals new { FirstName = x.FirstName, LastName = x.LastName }
                           select new
                           {
                               SQLID = sf.SQLID,
                               FirstName = x.FirstName,
                               LastName = x.LastName,
                               EmployeeId = x.EmployeeID
                           }).ToList();

                foreach(var emp in intersectEmployees)
                {
                    var _emp = context.Staff.Where(x => x.SQLID == emp.SQLID).FirstOrDefault();
                    _emp.ExtEmployeeID = emp.EmployeeId.ToString();
                    //_emp.STF_CODE = emp.EmployeeId.ToString();
                    await context.SaveChangesAsync();
                }


                return true;

            } catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<bool> PostAllEarningsRateIds(List<EarningsRate> rates)
        {

           try
            {
                foreach (var rate in rates)
                {
                    var itemtype = (from it in context.ItemTypes where it.AccountingIdentifier.ToLower() == rate.Name.ToLower() select it).ToList();
                    itemtype.ForEach(x => x.ExtPayID = rate.EarningsRateID.ToString());
                }
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return true;
        }

        public async Task<bool> ChangeDbConnection(string dbName, string connString)
        {
            DbManager.DbName = dbName;
            DbManager.ConnString = connString;
            return true;

            //var users = await (from ui in context.UserInfo where ui.Name == "SYSMGR" select new { ui.Name, ui.Password }).FirstOrDefaultAsync();
            //return users;
        }
    }
}
