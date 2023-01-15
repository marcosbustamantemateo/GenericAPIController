using Microsoft.AspNetCore.Identity;

namespace GenericControllerLib.Models
{
    /// <summary>
    ///     Clase que implementa el rol para estandar identity de .net
    /// </summary>
    [GeneratedController]
    public class Role : IdentityRole<int>
    {
        public DateTime? DeletedDate { get; set; }
    }
}
