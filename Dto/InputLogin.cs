using System.ComponentModel.DataAnnotations;

namespace GenericControllerLib.Dto
{
    /// <summary>
    ///     Clase que se usa como dto para loguearse en el sistema
    /// </summary>
    public class InputLogin
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}
