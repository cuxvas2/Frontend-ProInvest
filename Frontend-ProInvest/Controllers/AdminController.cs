using Microsoft.AspNetCore.Mvc;

namespace Frontend_ProInvest.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Menu()
        {
            return View();
        }
    }
}
