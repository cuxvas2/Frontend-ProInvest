using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class TipoInversionViewModel
    {
        public int IdTipo { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Tipo de Inversión")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Rendimiento { get; set; }
    }
}
