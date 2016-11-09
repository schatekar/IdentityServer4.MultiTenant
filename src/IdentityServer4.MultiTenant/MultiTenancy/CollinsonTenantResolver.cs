using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;

namespace IdentityServer4.MultiTenant.MultiTenancy
{
    public class CollinsonTenantResolver : ITenantResolver<CollinsonTenant>
    {
        private readonly IEnumerable<CollinsonTenant> _tenants = new List<CollinsonTenant>
        {
            new CollinsonTenant
            {
                Name = "mci",
                Hostname = "identity.mci.com:5000",
                Identity = new IdentityConfig
                {
                    Authority = "http://identity.mci.com:5000"
                }
            },
            new CollinsonTenant
            {
                Name = "amex",
                Hostname = "identity.amex.com:5001",
                Identity = new IdentityConfig
                {
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