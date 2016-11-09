using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;

namespace product1.MultiTenancy
{
    public class CollinsonTenantResolver : ITenantResolver<CollinsonTenant>
    {
        private readonly IEnumerable<CollinsonTenant> _tenants = new List<CollinsonTenant>
        {
            new CollinsonTenant
            {
                Name = "mci",
                Hostname = "product1.mci.com:6001",
                Identity = new IdentityConfig
                {
                    ClientId = "product1.mci.com",
                    ClientSecret = "product1",
                    Authority = "http://identity.mci.com:5000"
                }
            },
            new CollinsonTenant
            {
                Name = "amex",
                Hostname = "product1.amex.com:7001",
                Identity = new IdentityConfig
                {
                    ClientId = "product1.amex.com",
                    ClientSecret = "product1",
                    Authority = "http://identity.amex.com:5001"
                }
            }
        };

        public async Task<TenantContext<CollinsonTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<CollinsonTenant> tenantContext = null;

            var tenant =
                _tenants.FirstOrDefault(t => t.Hostname.Equals(context.Request.Host.Value.ToLower()));

            if (tenant != null)
            {
                tenantContext = new TenantContext<CollinsonTenant>(tenant);
            }

            return tenantContext;
        }
    }
}