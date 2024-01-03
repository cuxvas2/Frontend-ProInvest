using Frontend_ProInvest.Models;
using Newtonsoft.Json;

namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class InversionistaRespuestaJson
    {
        public List<InversionistaViewModel> Colonias { get; set; }
        public List<InversionistaViewModel> Inversionistas { get; set; }
        public InversionistaViewModel[] InversionistaNuevo { get; set; }
        public InversionistaViewModel Inversionista { get; set; }
        public string Token { get; set; }
    }
}
