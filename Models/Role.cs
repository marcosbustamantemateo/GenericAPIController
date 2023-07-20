using Microsoft.AspNetCore.Identity;

namespace GenericControllerLib.Models
{
    /// <summary>
    ///     Clase que implementa el rol para el estandar de seguridad identity
    /// </summary>
    [GeneratedController]
    public class Role : IdentityRole<int>
    {
        public DateTime? DeletedDate { get; set; }
	}
}
