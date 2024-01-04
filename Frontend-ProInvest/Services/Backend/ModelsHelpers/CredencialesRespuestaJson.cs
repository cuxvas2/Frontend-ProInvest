using Frontend_ProInvest.Models;
using Newtonsoft.Json;
using System.Net;

namespace Frontend_ProInvest.Services.Backend.ModelsHelpers
{
    public class CredencialesRespuestaJson
    {
        public string Usuario { get; set; }
        public string Token { get; set; }
        public HttpStatusCode  CodigoStatus { get; set; }
    }
}
