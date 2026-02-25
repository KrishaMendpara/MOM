using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult DashboardPage()
        {
            return View();
        }
    }
}
