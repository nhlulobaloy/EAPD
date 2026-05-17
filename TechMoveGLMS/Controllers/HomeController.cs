using Microsoft.AspNetCore.Mvc;

namespace TechMoveGLMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}