using Microsoft.AspNetCore.Mvc;

namespace TestAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}