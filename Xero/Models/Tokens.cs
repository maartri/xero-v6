namespace Xero.Models
{
    public class Tokens
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public List<Tenant> Tenants { get; set; }
    }
}
