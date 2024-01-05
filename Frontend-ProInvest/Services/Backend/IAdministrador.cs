using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.Net;
namespace Frontend_ProInvest.Services.Backend;

public interface IAdministrador
{
    public Task<CredencialesRespuestaJson> ObtenerCredencialesAccesoAsync(CredencialesAccesoViewModel credencialesAcceso);
    public Task<IEnumerable<BancosViewModel>> ObtenerBancos(string token);
    public Task<HttpStatusCode> RegistrarBanco(string nombreBanco, string token);
    public Task<HttpStatusCode> EditarBanco(BancosViewModel bancoEditado, string token);
    public Task<HttpStatusCode> EliminarBanco(int idBanco, string token);
    public Task<IEnumerable<TipoInversionViewModel>> GetTiposInversionAsync(string accessToken);
    public Task<bool> AnadirTiposInversionAsync(string accessToken, TipoInversionViewModel inversion);
    public Task<TipoInversionViewModel> GetTipoInversionAsync(string token, int id);
    public Task<bool> EditarTipoInversionAsync(string token, TipoInversionViewModel inversion);
    public Task<bool> EliminarTipoInversionAsync(string token, int id);
    public Task<OrigenInversionRespuestaJson> ObtenerOrigenesInversion(string token);
    public Task<IEnumerable<InformacionContrato>> ObtenerContratos(string token);
    public Task<SolicitudInversionViewModel> ObtenerSolicitudInversion(string token, InformacionContrato contrato);
    public Task<InformacionContrato> ObtenerInformacionContratoPorFolio(string token, int folio);
    public Task<IEnumerable<DocumentosExpedienteViewModel>> ObtenerDocumentosExpediente(string token);
    public Task<HttpStatusCode> RegistrarOrigenInversion(string origenInversion, string token);
    public Task<HttpStatusCode> EditarOrigenInversion(OrigenInversionViewModel origenEditado, string token);
    public Task<HttpStatusCode> EliminarOrigenInversion(int idOrigenInversion, string token);
    public Task<HttpStatusCode> RegistrarDocumento(string documentoExpediente, string token);
    public Task<HttpStatusCode> EditarDocumento(DocumentosExpedienteViewModel documentoExpediente, string token);
    public Task<HttpStatusCode> EliminarDocumento(int idDocumento, string token);
}
