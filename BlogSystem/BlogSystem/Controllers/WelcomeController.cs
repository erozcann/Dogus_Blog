using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
