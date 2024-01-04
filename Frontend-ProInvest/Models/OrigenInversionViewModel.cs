using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class OrigenInversionViewModel
    {
        public int IdOrigen { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa un origen de inversión válido (solo letras y números).")]
        [Display(Name = "Origen de Inversión")]
        public string NombreOrigen { get; set; }
    }
}
