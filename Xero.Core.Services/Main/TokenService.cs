using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Core.Data.Entities;
using Xero.Core.Services.Interfaces;
using Xero.NetStandard.OAuth2.Client;

using Microsoft.Extensions.Configuration;
using Xero.Core.Services.Helper;

namespace Xero.Core.Services.Main
{
    public class TokenService: ITokenService
    {
        private IMemoryCache _cache;
        private readonly IConfiguration Configuration;

        private string CLIENT_ID;
        private string SCOPES;
        private string CODE_VERIFIER;

        private string APPLICATION_NAME;
        private string REDIRECT_URI;

        private readonly string LOGPATH;
        private readonly string OUTPUTFILEPATH;

        public TokenService(
                IMemoryCache cache,
                IConfiguration configuration
            )
        {

            _cache = cache;
            Configuration = configuration;

            CLIENT_ID =         Configuration["XeroConfiguration:ClientId"].ToString();
            SCOPES =            Configuration["XeroConfiguration:Scopes"].ToString();
            CODE_VERIFIER =     Configuration["XeroConfiguration:CodeVerifier"].ToString();
            APPLICATION_NAME =  Configuration["XeroConfiguration:ApplicationName"].ToString();
            REDIRECT_URI =      Configuration["XeroConfiguration:REDIRECT_URI"].ToString();

            LOGPATH = Environment.CurrentDirectory;
            OUTPUTFILEPATH = Path.Combine(LOGPATH, "Output", "token.json");
        }

        public async Task<Tokens> GetTokensOnFile()
        {
            return await JsonFileReader.ReadAsync<Tokens>(this.OUTPUTFILEPATH);
        }

        public bool PostTokensOnFile(Tokens token)
        {
            var serializeJSON = Newtonsoft.Json.JsonConvert.SerializeObject(token);
            using (var writer = System.IO.File.CreateText(OUTPUTFILEPATH))
            {
                writer.WriteLine(serializeJSON);
            }

            return true;
        }

        public async Task<Tokens> GetRefreshTokens()
        {
            const string url = "https://identity.xero.com/connect/token";
            var client = new HttpClient();

            Tokens token = await GetTokensOnFile();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", CLIENT_ID),
                new KeyValuePair<string, string>("refresh_token", token.RefreshToken),
            });

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));

            var response = await client.PostAsync(url, formContent);
            var content = await response.Content.ReadAsStringAsync();
            var tokens = JObject.Parse(content);

            string? access_token = tokens["access_token"].ToString();
            _cache.Set("access_token", access_token, cacheEntryOptions);

            string? refresh_token_result = tokens["refresh_token"].ToString();
            _cache.Set("refresh_token", refresh_token_result, cacheEntryOptions);

            token.AccessToken = access_token;
            token.RefreshToken = refresh_token_result;

            PostTokensOnFile(token);

            return new Tokens()
            {
                IdToken = token.IdToken,
                AccessToken = access_token,
                RefreshToken = refresh_token_result
            };
        }
    }
}
