using Frontend_ProInvest.Models;
using Newtonsoft.Json;

namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class DireccionRespuestaJson
    {
        public List<DireccionViewModel> Colonias { get; set; }
        public string Token { get; set; }
    }
}
