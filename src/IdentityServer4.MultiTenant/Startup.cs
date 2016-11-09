using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.MultiTenant.InMemory;
using IdentityServer4.MultiTenant.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.MultiTenant
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<CollinsonTenant, CollinsonTenantResolver>();

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryUsers(Users.Get())
                .AddInMemoryScopes(new List<Scope>
                {
                    StandardScopes.OpenId,
                    StandardScopes.Profile,
                    StandardScopes.OfflineAccess,

                    new Scope
                    {
                        Name = "product1",
                        Description = "Product 1"
                    }
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client
                    {
                        ClientId = "product1.mci.com",
                        ClientName = "Prduct 1 MasterCard",
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        RequireConsent = false,

                        RedirectUris = {"http://product1.mci.com:6001/signin-oidc"},
                        PostLogoutRedirectUris = {"http://product1.mci.com:6001"},

                        ClientSecrets =
                        {
                          new Secret("product1".Sha256())
                        },

                        AllowedScopes = new List<string>
                        {
                            StandardScopes.OpenId.Name,
                            StandardScopes.Profile.Name
                        }
                    },
                    new Client
                    {
                        ClientId = "product1.amex.com",
                        ClientName = "Prduct 1 Amex",
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        RequireConsent = false,

                        RedirectUris = {"http://product1.amex.com:7001/signin-oidc"},
                        PostLogoutRedirectUris = {"http://product1.amex.com:7001"},

                        ClientSecrets =
                        {
                          new Secret("product1".Sha256())
                        },

                        AllowedScopes = new List<string>
                        {
                            StandardScopes.OpenId.Name,
                            StandardScopes.Profile.Name
                        }
                    }
                });

            services.AddMvc();
            //services.Configure<RazorViewEngineOptions>(opts =>
            //    opts.FileProviders.Add(
            //        new DatabaseFileProvider(Configuration["ConnectionString:DefaultConnection"])
            //    )
            //);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();



           

            app.UseMultitenancy<CollinsonTenant>();
            app.UsePerTenant<CollinsonTenant>((ctx, builder) =>
            {
                builder.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    AutomaticAuthenticate = false,
                    AutomaticChallenge = false,
                    CookieName = $"{ctx.Tenant.Name}.Identity"
                });

                //builder.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                //{
                //    AuthenticationScheme = "oidc",
                //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                //    SignOutScheme = IdentityServerConstants.SignoutScheme,
                //    DisplayName = $"{ctx.Tenant.Name} Identity Server",
                //    Authority = $"{ctx.Tenant.Identity.Authority}",
                //    ClientId = "implicit",
                //    ResponseType = "id_token",
                //    Scope = { "openid profile" },
                //    TokenValidationParameters = new TokenValidationParameters
                //    {
                //        NameClaimType = "name",
                //        RoleClaimType = "role"
                //    }
                //});



                builder.UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Account}/{action=Login}/{id?}");
                });
            });
        }
    }
}
