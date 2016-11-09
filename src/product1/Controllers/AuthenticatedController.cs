using Microsoft.AspNetCore.Mvc;

namespace product1.Controllers
{
    public class AuthenticatedController : Controller
    {
        public IActionResult Index()
        {
            return new EmptyResult();
        }

    }
}