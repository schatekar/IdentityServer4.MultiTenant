using System;
using System.Threading.Tasks;
using IdentityServer4.MultiTenant.MultiTenancy;
using IdentityServer4.MultiTenant.ViewModels;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.MultiTenant.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly CollinsonTenant _tenant;
        private readonly InMemoryUserLoginService _loginService;

        public AccountController(IIdentityServerInteractionService interaction, CollinsonTenant tenant, InMemoryUserLoginService loginService)
        {
            _interaction = interaction;
            _tenant = tenant;
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var model = new LoginViewModel
            {
                Tenant = _tenant.Name,
                ReturnUrl = returnUrl
            };
            return View(model);
        }



        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (_loginService.ValidateCredentials(model.Username, model.Password))
                {
                    // issue authentication cookie with subject ID and username
                    var user = _loginService.FindByUsername(model.Username);

                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                        };
                    };

                    await HttpContext.Authentication.SignInAsync(user.Subject, user.Username, props);

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // something went wrong, show form with error
            var vm = new LoginViewModel
            {
                Tenant = model.Tenant,
                Username = model.Username,
                ReturnUrl = model.ReturnUrl
            };
            return View(vm);
        }
    }
}