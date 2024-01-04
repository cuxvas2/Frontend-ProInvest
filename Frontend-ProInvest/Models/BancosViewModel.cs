using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class BancosViewModel
    {
        public int IdBanco { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre del Banco")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9&]+$", ErrorMessage = "Ingresa un banco válido sin caracteres especiales.")]
        public string Banco { get; set; }
        
    }
}
