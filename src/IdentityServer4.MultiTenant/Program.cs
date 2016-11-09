using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServer4.MultiTenant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://identity.mci.com:5000", "http://identity.amex.com:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
