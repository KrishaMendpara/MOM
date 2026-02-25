using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult AttendanceList()
        {
            return View();
        }
    }
}