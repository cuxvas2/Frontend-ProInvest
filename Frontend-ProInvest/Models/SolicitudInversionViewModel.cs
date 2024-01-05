using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Frontend_ProInvest.Models
{
    public class SolicitudInversionViewModel
    {
        public HttpStatusCode CodigoSolicitud { get; set; }

        public InversionistaViewModel Inversionista { get; set; }
        public InformacionBancariaViewModel InformacionBancaria { get; set; }
        public InformacionContrato InformacionContrato { get; set; }

        public List<DocumentosExpediente> Documentos { get; set; }
    }
}
