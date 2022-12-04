using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xero.Core.Data.Entities;

namespace Xero.Core.Services.Interfaces
{
    public interface ITokenService
    {
        Task<Tokens> GetTokensOnFile();
        bool PostTokensOnFile(Tokens token);
        Task<Tokens> GetRefreshTokens();
    }
}
