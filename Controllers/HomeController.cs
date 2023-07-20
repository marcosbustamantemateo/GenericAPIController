using Microsoft.AspNetCore.Mvc;

namespace GenericControllerLib.Controllers
{
    /// <summary>
    ///     Primer controlador en cargar
    /// </summary>
    public class HomeController : Controller
    {
        public HomeController() { }

        /// <summary>
        ///     Método principal
        /// </summary>
        public IActionResult Index()
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