using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericControllerLib.BusinessLogic
{
	public record struct ObjectBL(object? data, int rowsAffected);

    /// <summary>
    ///     Clase que provee los métodos de la lógica de negocio común para cualquier entidad
    /// </summary>
    /// <typeparam name="T">Entidad a usar</typeparam>
    public class BusinessLogic<T> where T : class
    {

        private readonly EntitiesDbContext _entitiesDbContext;

        public BusinessLogic(EntitiesDbContext entitiesDbContext)
        {
            _entitiesDbContext = entitiesDbContext;
        }

        /// <summary>
        ///     Crea un registro en base de datos del tipo T
        /// </summary>
        /// <param name="entity">Entidad del tipo T a crear</param>
        /// <returns>Resultado de la operación</returns>
        public virtual ObjectBL Create(T entity)
        {
            try
            {
                var result = _entitiesDbContext.Add(entity).Entity;
                int rowsAffected = _entitiesDbContext.SaveChanges();
                return new ObjectBL(result, rowsAffected);
            }
            catch
            {
                throw;
            }
        }

		/// <summary>
		///     Devuelve un listado de registros de base de datos del tipo T
		/// </summary>
		/// <param name="page">Nº de página a mostrar. Si se introduce -1 se muestran todos los resultados sin paginar</param>
		/// <param name="pageSize">Nº de resltados a mostrar por página</param>
		/// <param name="filter">Filtra por todas y cada una de las propiedas de la entidad</param>
		/// <param name="includeDeleted">Indica si se incluyen los elementos dados de baja</param>
		/// <param name="excludeActived">Indica si se excluyen los elementos dados de alta</param>
		/// <param name="includes">Incluye las propiedas de la entidad seguidas por comas</param>
		/// <returns>Listado de tipo T</returns>
		public virtual ObjectBL Read(int page, int pageSize, string filter, bool includeDeleted, bool excludeActived, string includes)
        {
            try
            {
                var includesAux = includes.Split(",").ToList();
                includesAux.ForEach(i => i.Trim());
				includesAux = string.IsNullOrEmpty(includes) || string.IsNullOrEmpty(includesAux[0]) ? null : includesAux;

                var expression = !includeDeleted 
                    ? GetFilterExpression(new Dictionary<string, object> { ["deletedDate"] = null }) 
                    : null;
                if (excludeActived)
					expression = includeDeleted
					    ? GetFilterExpression(new Dictionary<string, object> { ["!deletedDate"] = null })
					    : throw new Exception("No se pueden excluir los objetos activos si no se incluyen los dados de baja");
                var result = Get(expression, page, pageSize, filter, includesAux);
                return new ObjectBL(result, result.Queryable.Count());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Actualiza en base de datos un registro del tipo T
        /// </summary>
        /// <param name="entity">Entidad del tipo T a actalizar</param>
        /// <returns></returns>
        public virtual ObjectBL Update(T entity)
        {
            try
            {
                var result = _entitiesDbContext.Update(entity).Entity;
                int rowsAffected = _entitiesDbContext.SaveChanges();
                return new ObjectBL(result, rowsAffected);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Elimina o da de baja un registro del tipo T en base de datos dado su Id
        /// </summary>
        /// <param name="id">Id del objeto del tipo T a buscar</param>
        /// <param name="saveData">Indica si en lugar de borrar el objeto solo da de baja el objeto</param>
        /// <returns>Resultado de la operación</returns>
        public virtual ObjectBL Delete(int id, bool saveData)
        {
            try
            {
                var entity = GetByKey("id", id);
                object result = null;
                if (saveData)
                {
                    SetProperty(entity, "data.DeletedDate", DateTime.Now);
                    result = _entitiesDbContext.Update(entity.data).Entity;
                }
                else
                {
                    result = _entitiesDbContext.Remove(entity.data).Entity;
                }
                int rowsAffected = _entitiesDbContext.SaveChanges();
                return new ObjectBL(result, rowsAffected);
            }
            catch
            {
                throw;
            }
        }

		/// <summary>
		///     Da de alta un registro del tipo T en base de datos dado su Id
		/// </summary>
		/// <param name="id">Id del objeto del tipo T a buscar</param>
		/// <returns>Resultado de la operación</returns>
		public virtual ObjectBL Recover(int id)
		{
			try
			{
				var entity = GetByKey("id", id);
				SetProperty(entity, "data.DeletedDate", null);
				var result = _entitiesDbContext.Update(entity.data).Entity;
				int rowsAffected = _entitiesDbContext.SaveChanges();
				return new ObjectBL(result, rowsAffected);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///     Devuelve un registro del tipo T en base de datos dado su clave y valor dados
		/// </summary>
		/// <param name="key">Clave del objeto del tipo T a buscar</param>
		/// <param name="value">Valor del objeto del tipo T a buscar</param>
		/// <returns>Objeto del tipo T por su Código</returns>
		public ObjectBL GetByKey(string key, object value)
        {
            try
            {
                var typeGeneric = typeof(T);
                var property = typeGeneric.GetProperties().Where(i => i.Name.ToUpper() == key.ToUpper()).FirstOrDefault();

                if (property == null)
                    throw new Exception($"No se encuentra la propiedad {key} en el objeto {typeof(T)}");

                var propertyType = property.PropertyType;
                object? valueAux = Convert.ChangeType(value, propertyType);

                var expression = GetFilterExpression(new Dictionary<string, object> { [key] = valueAux });
                var result = Get(expression).Queryable.FirstOrDefault();
                return new ObjectBL(result, 1);
            }
            catch
            {
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Settea valores en propiedades anidadas de objetos genéricos
        /// </summary>
        /// <param name="target">Objeto en el cual se setteará el valor de la propiedad</param>
        /// <param name="compoundProperty">Cadena de texto que indica la anidación de las propiedades</param>
        /// <param name="value">Valor a settear</param>
        public void SetProperty(object target, string compoundProperty, object value)
        {
            try
            {
                string[] bits = compoundProperty.Split('.');
                for (int i = 0; i < bits.Length - 1; i++)
                {
                    PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                    if (propertyToGet != null)
                        target = propertyToGet.GetValue(target, null);
                }
                PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
                if (propertyToSet != null)
                    propertyToSet.SetValue(target, value);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Devuelve la expresión lambda necesaria para llamar al método Get(Expression<Func<T, bool>>? predicate = null)
        ///     ségún el diccionario con las claves y valores dados
        /// </summary>
        /// <param name="properties2Values"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>>? GetFilterExpression(Dictionary<string, object> properties2Values)
        {
            try
            {
                ParameterExpression argParam = Expression.Parameter(typeof(T), "s");

                if (properties2Values.Count == 1)
                {
                    if (properties2Values.ElementAt(0).Key.Contains("!"))
                    {
						Expression nameProperty = Expression.Property(argParam, properties2Values.ElementAt(0).Key.Replace("!", ""));
						ConstantExpression constantExpression = Expression.Constant(properties2Values.ElementAt(0).Value);
						Expression expression = Expression.NotEqual(nameProperty, constantExpression);

						return Expression.Lambda<Func<T, bool>>(expression, argParam);
					}
                    else
                    {
                        Expression nameProperty = Expression.Property(argParam, properties2Values.ElementAt(0).Key);
                        ConstantExpression constantExpression = Expression.Constant(properties2Values.ElementAt(0).Value);
                        Expression expression = Expression.Equal(nameProperty, constantExpression);

                        return Expression.Lambda<Func<T, bool>>(expression, argParam);
                    }
                }
                // TO DO: No está bien hecho para cuando hay más de un parámetro
                else if (properties2Values.Count > 1)
                {
                    List<BinaryExpression> binaryExpression = new List<BinaryExpression>();
                    for (int i = 0; i < properties2Values.Count; i++)
                    {
                        Expression nameProperty = Expression.Property(argParam, properties2Values.ElementAt(i).Key);
                        ConstantExpression constantExpression = Expression.Constant(properties2Values.ElementAt(i).Value);

                        Expression expression = Expression.Equal(nameProperty, constantExpression);
                        binaryExpression.Add((BinaryExpression)expression);
                    }
                    return Expression.Lambda<Func<T, bool>>(binaryExpression[0], argParam);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

		/// <summary>
		/// Devuelve un listado de base de datos en función del predicado
		/// </summary>
		/// <param name="predicate">Predicado para filtrar</param>
		/// <param name="page">Nº de página a mostrar. Si se introduce -1 se muestran todos los resultados sin paginar</param>
		/// <param name="pageSize">Nº de resultados a mostrar por página</param>
		/// <param name="filter">Cadena de texto para filtrar por todas y cada una de las propiedas de la entidad</param>
		/// <param name="includes">Incluye las propiedas de la entidad seguidas por comas</param>
		/// <returns>Listado de base de datos</returns>
		private PagedResult<T> Get(Expression<Func<T, bool>>? predicate = null, int page = -1, int pageSize = 10, string filter = "", List<string>? includes  = null)
        {
            try
            {
                IQueryable<T>? list = null;
                var skip = (page - 1) * pageSize;

                // Se obtiene todas las propiedades del tipo genérico
                PropertyInfo[] properties = typeof(T).GetProperties();

                // Se crea una lista de expresiones para todos los parámetros del objeto T para añadir a la query mediante un where
                var filterExpression = BuildStringFilterExpression(filter);

                IQueryable<T> aux = _entitiesDbContext
                    .Set<T>()
                    .Where(filterExpression)
                    .AsQueryable();

                if (page == -1)
                    list = predicate == null ? aux : aux.Where(predicate);
                else
                    list = predicate == null ? aux.Skip(skip).Take(pageSize) : aux.Skip(skip).Take(pageSize).Where(predicate);

                // Se incluyen las entidades relacionadas
                if (includes != null)
                {
                    list = Util.Include(list, includes);
                }

                return new PagedResult<T>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    RowCount = list.Count(),
                    Queryable = list,
                    PageCount = (int) Math.Ceiling((double) list.Count() / pageSize)
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Construye una expresión lambda que filtra según una cadena de texto dada recorriendo los valores de todas las propiedades 
        ///     de tipo string del objeto tipo T.
        /// </summary>
        /// <param name="filter">Texto a filtrar</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> BuildStringFilterExpression(string filter)
        {
            try
            {
                var param = Expression.Parameter(typeof(T));

                var properties = typeof(T).GetProperties();
                var conditions = new List<Expression>();

                foreach (var property in properties)
                {
                    // Crea una expresión del tipo "property == filter"
                    var propertyAccess = Expression.MakeMemberAccess(param, property);
                    var filterConstant = Expression.Constant(filter);

                    if (propertyAccess.Type == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var contains = Expression.Call(propertyAccess, containsMethod, filterConstant);
                        // var equal = Expression.Equal(propertyAccess, filterConstant);
                        conditions.Add(contains);
                    } 
                }

                // Se agrupa las expresiones en una expresión "conditions[0] || conditions[1] || conditions[2] || ... || conditions[n]"
                var body = conditions.Aggregate((accumulate, next) => Expression.Or(accumulate, next));

                // Se crea y devuelve la expresión lambda final
                return Expression.Lambda<Func<T, bool>>(body, param);
            }
            catch
            {
                throw;
            }
        }

    #endregion
    }    
}
