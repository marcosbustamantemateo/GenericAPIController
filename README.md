# Generic API Controller

Tired of always doing the same to start a project?<br>
This is for you! The perfect solution for starting .NET Core API Projects. <br>

Provides and generates API controllers and business logic CRUD methods for any object you want with the GeneratedController annotation.
Include Swagger Documentation API with JWT authentication.

# Documentation

- Generic Controller Base:

  ```cs class:"lineNo"
  /// <summary>
    ///     Controlador base que proporciona métodos CRUD para cualquier tipo de objeto
    /// </summary>
    /// <typeparam name="T">Entidad tipo T</typeparam>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(BaseDto), 200)]
    [ProducesResponseType(typeof(BaseDto), 400)]
    [ProducesResponseType(typeof(BaseDto), 401)]
    [ProducesResponseType(typeof(BaseDto), 500)]
    public class BaseAPIController<T> : ControllerBase where T : class
    {
        private readonly BusinessLogic<T> _businessLogic;

        public BaseAPIController(BusinessLogic<T> businessLogic)
        {
            _businessLogic = businessLogic;
        }

        /// <summary>
        ///     Crea una nueva entidad
        /// </summary>
        /// <param name="entity">Objeto a crear</param>
        /// <returns>Resultado de la operación</returns>
        [Authorize(Roles = "Superadmin, Admin")]
        [HttpPost("Create_Generic")]
        public async Task<IActionResult> Create(T entity)
        {
            try
            {
                if (entity != null) 
                {
                    var result = _businessLogic.Create(entity);
                    return Ok(new BaseDto($"¡Objeto creado con éxito!", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El objeto enviado es nulo o no coincide con la definición!", null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo crear!", ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
        }

        /// <summary>
        ///     Devuelve un listado de entidad dada
        /// </summary>
        /// <param name="page">Nº de página a mostrar. Si se introduce -1 se muestran todos los resultados sin paginar</param>
        /// <param name="pageSize">Nº de resltados a mostrar por página</param>
        /// <param name="filter">Cadena de texto para filtrar por todas y cada una de las propiedas de la entidad</param>
        /// <param name="includeDeleted">Indica si se incluyen los elementos dados de baja</param>
        /// <returns>Listado de entidad dada</returns>
        [HttpGet("Read_Generic")]
        public async Task<ActionResult> Read(int page = 1, int pageSize = 10, string filter = "", [Required] bool includeDeleted = true)
        {
            try
            {
                if (page >= 1 && pageSize >= 1)
                {
                    var result = _businessLogic.Read(page, pageSize, filter, includeDeleted);
                    var pagedResultAux = (PagedResult<T>)result.data;

                    return Ok(new BaseDto($"Objeto tipo: {typeof(T)}" +
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
        ///     Actualiza una entidad
        /// </summary>
        /// <param name="entity">Objeto a actualizar</param>
        /// <returns>Resultado de la operación</returns>
        [Authorize(Roles = "Superadmin, Admin")]
        [HttpPut("Update_Generic")]
        public async Task<IActionResult> Update(T entity)
        {
            try
            {
                if (entity != null)
                {

                    var result = _businessLogic?.Update(entity);
                    return Ok(new BaseDto($"¡Objeto del tipo {typeof(T)} actualizado con éxito!", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El objeto enviado es nulo o no coincide con la definición!", null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto("¡Error: No se pudo actualizar!", ex.Message));
            }
        }

        /// <summary>
        ///     Da de baja una entidad dado su ids
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <param name="saveData">Indica si en lugar de borrar el objeto solo da de baja el objeto</param>
        /// <returns>Resultado de la operación</returns>
        [Authorize(Roles = "Superadmin")]
        [HttpDelete("Delete_Generic")]
        public async Task<IActionResult> Delete(int id, bool saveData = true)
        {
            try
            {
                if (id > 0)
                {
                    var result = _businessLogic.Delete(id, saveData);
                    if (saveData)
                        return Ok(new BaseDto($"¡Objeto del tipo {typeof(T)} dado de baja con éxito!", result));
                    else 
                        return Ok(new BaseDto($"¡Objeto del tipo {typeof(T)} eliminado con éxito!", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El parámetro id no puede ser inferior a 1!", null));
                }
            }
            catch (Exception ex)
            {
                if (saveData)
                    return StatusCode(500, new BaseDto("¡Error: No se pudo dar de baja!", ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                else
                    return StatusCode(500, new BaseDto("¡Error: No se pudo eliminar!", ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
        }

        /// <summary>
        ///     Devuelve una entidad dada su clave y valor
        /// </summary>
        /// <param name="key">Propiedad de la entidad a filtrar</param>
        /// <param name="value">Valor de la propiedad a filtrar</param>
        /// <returns>Entidad dada</returns>
        [HttpGet("ReadByKey_Generic")]
        public async Task<ActionResult> ReadByKey(string key, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(key))
                {
                    var result = _businessLogic.GetByKey(key, value);
                    return Ok(new BaseDto($"Objeto tipo: {typeof(T)}", result));
                }
                else
                {
                    return BadRequest(new BaseDto("¡Error: El parámetro key y/o value no pueden estar vacios!", new { key, value }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseDto($"¡Error: No se pudo encontrar la entidad donde el parámetro '{key}' es igual a '{value}'!", ex.Message));
            }
        }
    }
  ```
