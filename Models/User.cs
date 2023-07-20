using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericControllerLib.Models
{
    /// <summary>
    ///     Clase que implementa el usuario para el estandar de seguridad identity
    /// </summary>
    [GeneratedController]
    public class User : IdentityUser<int>
    {
        public DateTime? DeletedDate { get; set; }

		[NotMapped]
		public List<string>? Roles { get; set; }       
    }
}
