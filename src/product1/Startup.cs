using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using product1.MultiTenancy;

namespace product1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<CollinsonTenant, CollinsonTenantResolver>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMultitenancy<CollinsonTenant>();
            app.UsePerTenant<CollinsonTenant>((ctx, builder) =>
            {
                builder.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "Cookies",
                    CookieName = $"{ctx.Tenant.Name}.AspNet.Cookies"
                });

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


                builder.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = "oidc",
                    SignInScheme = "Cookies",

                    Authority = ctx.Tenant.Identity.Authority,
                    RequireHttpsMetadata = false,

                    ClientId = ctx.Tenant.Identity.ClientId,
                    ClientSecret = ctx.Tenant.Identity.ClientSecret,
                    ResponseType = "code id_token",

                    SaveTokens = true,
//                    Events = new OpenIdConnectEvents
//                    {
//                        OnRedirectToIdentityProvider = redirectContext =>
//                        {
//                            redirectContext.ProtocolMessage.AcrValues = $"tenant:{currentIdentityConfig?.Tenant}";
//                            return Task.FromResult(0);
//                        }
//                    }
                });

                builder.UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
            });


        }
    }


}
