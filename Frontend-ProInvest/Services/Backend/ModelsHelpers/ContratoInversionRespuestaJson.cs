using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class ContratoInversionRespuestaJson
    {
        public ContratoInversionModel InformacionContrato { get; set; }
        public List<ContratoInversionModel> ContratoActualizado { get; set; }
        public string Token { get; set; }
    }
}
