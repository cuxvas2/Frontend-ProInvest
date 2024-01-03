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
}
