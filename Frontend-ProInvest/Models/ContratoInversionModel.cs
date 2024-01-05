using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class ContratoInversionModel
    {
        public string DireccionIp { get; set; }
        public string Contrato { get; set; }
        public int? IdInversionista { get; set; }
        public int? IdTipo { get; set; }
        public int? IdOrigen { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Ingresa tu folio de inversión (sólo números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Ingresa tu folio de inversión")]
        public int? FolioInversion { get; set; }
        public DateTime? Fecha { get; set; }
        public double? Importe { get; set; }
        public int? PlazoAnios { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
        public bool? SmsVerificacion { get; set; }
        public bool? CorreoVerificacion { get; set; }
        public string Estado { get; set; }

    }
}

