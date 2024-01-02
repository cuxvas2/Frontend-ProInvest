using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;

namespace Frontend_ProInvest.Services.Backend;

public interface IAdministrador
{
    public Task<CredencialesRespuestaJson> ObtenerCredencialesAccesoAsync(CredencialesAccesoViewModel credencialesAcceso);
}
