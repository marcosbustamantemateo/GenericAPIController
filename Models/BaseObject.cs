using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericControllerLib.Models
{
    /// <summary>
    ///     Anotación que genera un controlador de forma dinámica con los
    ///     métodos CRUD
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GeneratedControllerAttribute : Attribute
    {
        public GeneratedControllerAttribute() { }
    }

    /// <summary>
    ///     Objeto base con propiedades comunes en todas las entidades
    ///     del proyecto
    /// </summary>
    public class BaseObject
    {
        /// <summary>
        ///     Identificador del objeto.
        /// </summary>
        [Display(Name = "Id")]
        [Key]
        [Column("INT_ID", Order = 0, TypeName = "INT")]
        public int Id { get; set; }

        /// <summary>
        ///     Fecha de alta de creación del objeto
        /// </summary>
        [Display(Name = "Fecha de Alta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("DAT_ALTA", Order = 1, TypeName = "DATETIME2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        ///     Fecha de baja del objeto
        /// </summary>
        [Display(Name = "Fecha de Baja")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("DAT_BAJA", Order = 1, TypeName = "DATETIME2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DeletedDate { get; set; }

        /// <summary>
        ///     Código del objeto.
        /// </summary>
        [MaxLength(150, ErrorMessage = "La longitud máxima permitida es de 150 carácteres")]
        [Display(Name = "Código")]
        [Column("VAR_CODIGO", Order = 2, TypeName = "NVARCHAR(150)")]
        public string? Code { get; set; }

        /// <summary>
        ///     Descripción del objeto.
        /// </summary>
        [MaxLength(3000, ErrorMessage = "La longitud máxima permitida es de 3000 carácteres")]
        [Display(Name = "Descripción")]
        [Column("VAR_DESCRIPCION", Order = 3, TypeName = "NVARCHAR(200)")]
        public string? Description { get; set; }
    }
}
