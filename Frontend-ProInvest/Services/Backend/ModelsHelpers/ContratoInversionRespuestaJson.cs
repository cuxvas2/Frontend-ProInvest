namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class ContratoInversionRespuestaJson
    {
        public InformacionContrato InformacionContrato { get; set; }
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
    }
}
