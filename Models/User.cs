using Microsoft.AspNetCore.Identity;

namespace GenericControllerLib.Models
{
    /// <summary>
    ///     Clase que implementa el usuario para estandar identity de .net
    /// </summary>
    [GeneratedController]
    public class User : IdentityUser<int>
    {
        public DateTime? DeletedDate { get; set; }
        public List<string>? Roles { get; set; }       
    }
}
