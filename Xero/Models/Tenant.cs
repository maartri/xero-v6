namespace Xero.Models
{
    public class Tenant
    {
        public Tenant(string tenantId, string tenantName)
        {
            Id = tenantId;
            Name = tenantName;
        }

        public string Id { get; }

        public string Name { get; }
    }
}
