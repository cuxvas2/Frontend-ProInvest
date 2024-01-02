using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;

namespace Frontend_ProInvest.Services.Backend
{
    public interface IUsuarios
    {
        public Task<List<string>> GetEstadosAsync();
        public Task<List<InversionistaViewModel>> GetColoniasPorCodigoPostalAsync(string direccionIp, string codigoPostal);
        public Task<InversionistaViewModel> AnadirInformacionPersonalInversionistaAsync(InversionistaViewModel datosPersonales);
        public Task<bool> CrearContratoInversionAsync(string ip, int id, DateTime fechaActualizacion);
        public Task<ContratoInversionRespuestaJson> ObtenerContratoInversionPorIpAsync(string ip);
    }
}
