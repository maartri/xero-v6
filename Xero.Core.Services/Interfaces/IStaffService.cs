using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Core.Data.Entities;
using Xero.Core.Data.Dto;
using Xero.NetStandard.OAuth2.Model.PayrollAu;

namespace Xero.Core.Services.Interfaces
{
    public interface IStaffService
    {
        Task<List<Staff>> GetStaffs();
        Task<List<ItemTypesDto>> GetItemTypes(ItemTypes it);
        Task<bool> PostAllEarningsRateIds(List<EarningsRate> rates);
        Task<bool> PostXeroEmployeeId(Employees employees);
        Task<bool> ChangeDbConnection(string dbName, string connString);

    }
}
