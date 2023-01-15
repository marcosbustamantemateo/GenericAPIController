using Microsoft.AspNetCore.Mvc;

namespace GenericControllerLib.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Http(int statusCode)
        {
            if (statusCode == 401 || statusCode == 403)
                return StatusCode(401, "¡Error: Usuario no autenticado u autorizado!");

            if (statusCode == 404)
                return StatusCode(404, "¡Error: Recurso no encontrado!");

            return StatusCode(statusCode, $"¡Error {statusCode}!");
        }
    }
}
