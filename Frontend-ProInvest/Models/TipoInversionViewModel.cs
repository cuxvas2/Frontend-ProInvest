using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frontend_ProInvest.Models
{
    public class TipoInversionViewModel
    {
        public int IdTipo { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "El campo {0} debe ser una cadena con un minimo de {1} y un maximo de {2} caracteres")]
        [Display(Name = "Tipo de Inversión")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "Rendimiento anual")]
        [Range(1, 100, ErrorMessage = "El campo {0} debe ser un númeero con un minimo de {1} y un máximo de {2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public double Rendimiento { get; set; }
    }
}
