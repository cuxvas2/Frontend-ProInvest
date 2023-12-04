using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class InversionViewModel
    {
        [Required(ErrorMessage = "El campo Importe es obligatorio")]
        [RegularExpression(@"^[1-9]\d*0000$", ErrorMessage = "Ingrese un número entero y con aumentos de $10,000")]
        [Range(10000, 2000000, ErrorMessage = "El importe debe estar entre 10,000 y 2,000,000")]
        [Display(Name = "Ingresa tu importe desde $10,000")]
        public int Importe {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "¿En cuánto tiempo te gustaría obtener tus rendimientos?")]
        public int Plazo { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Tipo de Inversión")]
        public TipoInversionViewModel TipoInversion { get; set; }
    }
}
