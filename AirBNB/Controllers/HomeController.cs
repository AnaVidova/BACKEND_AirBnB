using Microsoft.AspNetCore.Mvc;

namespace AirBNB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
