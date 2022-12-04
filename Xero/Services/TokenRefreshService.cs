using System.Diagnostics;
using Xero.Core.Services.Interfaces;

namespace Xero.Services
{
    public class TokenRefreshService : BackgroundService
    {
        private readonly ITokenService _tokenS;

        public TokenRefreshService(ITokenService tokenS)
        {
            _tokenS = tokenS;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Debug.WriteLine(_tokenS.GetRefreshTokens());
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
