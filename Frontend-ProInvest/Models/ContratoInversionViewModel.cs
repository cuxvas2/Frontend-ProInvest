using System.Security.Cryptography.X509Certificates;

namespace Frontend_ProInvest.Models
{
    public class ContratoInversionViewModel
    {
        public string DireccionIp { get; set; }
        public string Contrato { get; set; }
        public int IdInversionista { get; set; }
        public int IdTipo { get; set; }
        public int IdOrigin { get; set; }
        public int FolioInversion { get; set; }
        public DateTime Fecha { get; set; }
        public double Importe { get; set; }
        public int PlazoAnios { get; set; }
        public string Estado { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public bool SmsVerificacion { get; set; }
        public bool CorreoVerificacion { get; set; }

        public InversionistaViewModel Inversionista { get; set; }
        public TipoInversionViewModel TipoInversion {  get; set; }

        public ContratoInversionViewModel()
        {
            Inversionista = new InversionistaViewModel();
            TipoInversion = new TipoInversionViewModel();
        }
    }
}
