using Microsoft.AspNetCore.Mvc;
using MSLSystem.Services;

namespace MSLSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string E_MAIL, string PASSWORD)
        {
            var user = _userService.GetUserByEmail(E_MAIL);

            if (user != null && user.PASSWORD == PASSWORD)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            TempData["ErrorMessage"] = "Invalid email or password"; 
            return RedirectToAction("Index");
        }
    }
}
