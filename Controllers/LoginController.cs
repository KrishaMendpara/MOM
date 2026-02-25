using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult RegisterPage()
        {
            return View();
        }
    }
}
