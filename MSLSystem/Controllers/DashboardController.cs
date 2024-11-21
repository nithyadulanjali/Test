using Microsoft.AspNetCore.Mvc;

namespace MSLSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
