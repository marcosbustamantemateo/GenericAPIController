using Microsoft.AspNetCore.Mvc;

namespace GenericControllerLib.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() { }

        public async Task<IActionResult> Index()
        {
            try
            {
                return Redirect("~/swagger");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}