using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class DocumentosExpedienteViewModel
    {
        public int IdDocumento { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre del Documento")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9&]+$", ErrorMessage = "Ingresa un documento válido sin caracteres especiales.")]
        public string NombreDocumento { get; set; }
        
    }
}
