using Microsoft.AspNetCore.Mvc;

namespace GenericControllerLib.Controllers
{
    /// <summary>
    ///     Captura
    /// </summary>
    public class ErrorController : Controller
    {
		/// <summary>
		///     Maneja los errores según su código y devuelve sus respectivas vistas 
		/// </summary>
		/// <param name="statusCode">Código de estado del error</param>
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
