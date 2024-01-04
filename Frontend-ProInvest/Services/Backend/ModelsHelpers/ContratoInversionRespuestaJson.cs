using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class ContratoInversionRespuestaJson
    {
        public ContratoInversionModel InformacionContrato { get; set; }
        public List<ContratoInversionModel> ContratoActualizado { get; set; }
        public string Token { get; set; }
    }
    public class InformacionContrato
    {
        public string DireccionIp {  get; set; }
        public int IdInversionista { get; set; }
        public int FolioInversion {  get; set; }
        public string Estado {  get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public bool? SmsVerificacion {  get; set; }
        public bool? CorreoVerificacion { get; set;  }
        public int Importe { get; set; }
        public int IdTipo { get; set; }
        public string Contrato { get; set; }
    }
}
