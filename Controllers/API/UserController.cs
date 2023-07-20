using GenericControllerLib.BusinessLogic;
using GenericControllerLib.Dto;
using GenericControllerLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

namespace GenericControllerLib.Controllers.API
{
	/// <summary>
	///     Controlador base que proporciona métodos CRUD para el clase User
	/// </summary>
	[Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(BaseDto), 400)]
    [ProducesResponseType(typeof(BaseDto), 500)]
    public class UserController : Controller
    {
        private readonly UserBL _usuarioBL;

        public UserController(UserBL usuarioBL)
        {
            _usuarioBL= usuarioBL;
        }

		/// <summary>
		///     Devuelve un listado de entidad dada
		/// </summary>
		/// <param name="page">Nº de página a mostrar. Si se introduce -1 se muestran todos los resultados sin paginar</param>
		/// <param name="pageSize">Nº de resltados a mostrar por página</param>
		/// <param name="includes">Incluye las propiedas de la entidad seguidas por comas</param>
		/// <param name="filter">Filtra por todas y cada una de las propiedas de la entidad</param>
        /// <param name="includeDeleted">Indica si se incluyen los elementos dados de baja</param>
		/// <param name="excludeActived">Indica si se excluyen los elementos dados de alta</param>
		/// <returns>Listado de entidad dada</returns>
		//[Authorize]
		[HttpGet("Read")]
        public async Task<ActionResult> Read(int page = 1, int pageSize = 10, string? includes = "", string? filter = "",
			                                    [Required] bool includeDeleted = true, [Required] bool excludeActived = false)
        {
            try
            {
                if (page >= 1 || pageSize >= 1)
                {
                    var result = _usuarioBL.Read(page, pageSize, filter, includeDeleted, excludeActived, includes);
                    var pagedResultAux = (PagedResult<User>?)result.data;

                    return Ok(new BaseDto($"Objeto tipo: {typeof(User)}" +
                        $"Total elementos: {pagedResultAux.Queryable.Count()}. " +
                        (page != -1
                            ? ($"Elementos por página: {pagedResultAux.PageSize}. "
                                + $"Mostrando página {pagedResultAux.CurrentPage} de {pagedResultAux.PageCount}. ")
                            : $"Mostrando todos los elementos (sin paginación).")
                        , result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El parámetro page o pageSize no puede ser inferior a 1!", new { page, pageSize }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo listar!", ex.Message));
            }
        }

        /// <summary>
        ///     Devuelve el token que autentica y autoriza a las demás acciones de la API
        /// </summary>
        /// <param name="inputLogin">Usuario y clave a ingresar</param>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(BaseDto), 401)]
        [ProducesResponseType(typeof(BaseDto), 404)]
        public async Task<IActionResult> Login([FromBody] InputLogin inputLogin)
        {
            try
            {
                if (inputLogin != null)
                {
                    var login = await _usuarioBL.Login(inputLogin);

                    if (login != null)
                    {
                        if (login.Value.rowsAffected == 1)
                            return Ok($"Bearer {login.Value.data}");
                        else
                            return StatusCode(401, "¡Error: Usuario y/o clave incorrectos!");
                    }
                    else
                    {
                        return StatusCode(404, "¡Error: Usuario no encontrado");
                    }
                }
                else
                {
                    return BadRequest("Error: El parámetro usuario y/o clave no pueden estar vacios");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo obtener el token!", ex.Message));
            }
        }

        /// <summary>
        ///     Añade un rol a un usuario dados sus ids
        /// </summary>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="idRol">Id del rol</param>
        //[Authorize(Roles = "Superadmin")]
        [HttpPost("AssignRole")]
        [ProducesResponseType(typeof(BaseDto), 401)]
        public IActionResult AssignRole(int idUsuario, int idRol)
        {
            try
            {
                if (idUsuario > 0 && idRol > 0)
                {
                    var result = _usuarioBL.AssignRole(idUsuario, idRol);
                    return Ok(new BaseDto("¡Rol añadido al usuario con éxito", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El parámetro idUsuario o idRol no puede ser inferior a 1!", new { idUsuario, idRol }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo añadir el rol al usuario!", ex.Message));
            }
        }

        /// <summary>
        ///     Elimina un rol de un usuario dados sus ids
        /// </summary>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="idRol">Id del rol</param>
        [ProducesResponseType(typeof(BaseDto), 401)]
        //[Authorize(Roles = "Superadmin")]
        [HttpDelete("DeleteRole")]
        public IActionResult DeleteRole(int idUsuario, int idRol)
        {
            try
            {
                if (idUsuario > 0 && idRol > 0)
                {
                    var result = _usuarioBL.DeleteRole(idUsuario, idRol);
                    return Ok(new BaseDto("¡Rol elidelminado al usuario con éxito", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El parámetro idUsuario o idRol no puede ser inferior a 1!", new { idUsuario, idRol }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo eliminar el rol del usuario!", ex.Message));
            }
        }
    }
}
