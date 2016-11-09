using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using product1.MultiTenancy;

namespace product1.Controllers
{
    public class HomeController : Controller
    {
        private readonly CollinsonTenant _tenant;

        public HomeController(CollinsonTenant tenant)
        {
            _tenant = tenant;
        }
        [Authorize]
        public IActionResult Index()
        {
            var vm = new HomeViewModel
            {
                Tenant = _tenant.Name
            };
            return View(vm);
        }
    }

    public class HomeViewModel
    {
        public string Tenant { get; set; }
    }
}