using GenericControllerLib.Dto;
using GenericControllerLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;

namespace GenericControllerLib.BusinessLogic
{
	/// <summary>
	///     Clase que provee los métodos de la lógica de negocio específica para la clase User
	/// </summary>
	public class UserBL : BusinessLogic<User>
	{
		private readonly EntitiesDbContext _entitiesDbContext;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;

		public UserBL(EntitiesDbContext entitiesDbContext, UserManager<User> userManager,
			IConfiguration configuration, SignInManager<User> signInManager) : base(entitiesDbContext)
		{
			_entitiesDbContext = entitiesDbContext;
			_userManager = userManager;
			_configuration = configuration;
			_signInManager = signInManager;
		}

		/// <summary>
		///     Devuelve un listado de registros de base de datos del tipo User
		/// </summary>
		/// <param name="page">Nº de página a mostrar. Si se introduce -1 se muestran todos los resultados sin paginar</param>
		/// <param name="pageSize">Nº de resltados a mostrar por página</param>
		/// <param name="filter">Cadena de texto para filtrar por todas y cada una de las propiedas de la entidad</param>
		/// <param name="includeDeleted">Indica si se incluyen los elementos dados de baja</param>
		/// <param name="excludeActived">Indica si se excluyen los elementos dados de alta</param>
		/// <param name="includes">Incluye las propiedas de la entidad seguidas por comas</param>
		/// <returns>Listado de tipo User</returns>
		public override ObjectBL Read(int page, int pageSize, string filter, bool includeDeleted, bool excludeActived, string includes)
		{
			try
			{
				var result = base.Read(page, pageSize, filter, includeDeleted, excludeActived, includes);
				var users = (PagedResult<User>?)result.data;
				foreach (var user in users.Queryable)
				{
					var usersRoles = _entitiesDbContext.UserRoles
						.Where(i => i.UserId == user.Id)
						.Select(i => i.RoleId)
						.ToList();

					var roles = _entitiesDbContext.Roles
						.Where(i => usersRoles.Contains(i.Id))
						.Select(i => i.Name)
						.ToList();
					user.Roles = roles;
				}
				result.data = users;
				return result;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///     Permite al usuario con su contraseña ingresar al sistema si esta es correcta
		/// </summary>
		/// <param name="inputLogin">Modelo de usuario y contraseña</param>
		/// <returns>
		///     Devuelve el token generado si es correcto en el data de ObjectBL
		///     ObjectBL null si no se encuentra el usuario. Si se encuentra, RowsAffected 1 si coincide la 
		///     contraseña, 0 si es incorrecta
		/// </returns>
		public async Task<ObjectBL?> Login(InputLogin inputLogin)
		{
			try
			{
				var userDB = await _entitiesDbContext.Users
						.SingleOrDefaultAsync(u => u.UserName == inputLogin.Usuario);

				if (userDB != default(User))
				{
					var result = await _signInManager.CheckPasswordSignInAsync(userDB, inputLogin.Clave, false);

					if (result.Succeeded)
					{
						var userRoles = await _userManager.GetRolesAsync(userDB);
						var authClaims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, userDB.UserName),
							new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						};

						foreach (var userRole in userRoles)
						{
							authClaims.Add(new Claim(ClaimTypes.Role, userRole));
						}

						var jwtSecurityToken = GetToken(authClaims);
						var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

						return new ObjectBL(token, 1);
					}
					else
						return new ObjectBL(null, 0);
				}
				else
					return null;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///     Agrega un nuevo rol un usuario dados sus ids
		/// </summary>
		/// <param name="idUsuario">Id del usuario</param>
		/// <param name="idRol">Id del rol</param>
		/// <returns>Devuelve el objeto userRole creado</returns>
		public ObjectBL? AssignRole(int idUsuario, int idRol)
		{
			try
			{
				var identityUserRole = new IdentityUserRole<int>() { UserId = idUsuario, RoleId = idRol };
				_entitiesDbContext.UserRoles.Add(identityUserRole);
				_entitiesDbContext.SaveChanges();

				return new ObjectBL(identityUserRole, 1);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///     Borra un rol de un usuario dados sus ids
		/// </summary>
		/// <param name="idUsuario">Id del usuario</param>
		/// <param name="idRol">Id del rol</param>
		/// <returns>Devuelve el objeto userRole borrado</returns>
		public ObjectBL? DeleteRole(int idUsuario, int idRol)
		{
			try
			{
				var identityUserRole = _entitiesDbContext.UserRoles
					.SingleOrDefault(i => i.UserId == idUsuario && i.RoleId == idRol);

				_entitiesDbContext.UserRoles.Remove(identityUserRole);
				_entitiesDbContext.SaveChanges();

				return new ObjectBL(identityUserRole, 1);
			}
			catch
			{
				throw;
			}
		}

		#region Private methods

		/// <summary>
		///     Crea el token a partir de los parámetros de autenticación
		/// </summary>
		/// <param name="authClaims">Listado de permisos de autenticación</param>
		/// <returns></returns>
		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			return token;
		}

		#endregion
	}
}
