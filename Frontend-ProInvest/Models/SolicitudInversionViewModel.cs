using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class SolicitudInversionViewModel
    {
        [Display(Name = "Folio")]
        public int FolioInversion { get; set; }
        [Display(Name = "Nombre")]
        public string NombreCompleto { get; set; }
        public string Estado { get; set; }
    }
}
