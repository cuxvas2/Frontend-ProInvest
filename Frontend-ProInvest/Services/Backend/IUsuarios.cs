using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;

namespace Frontend_ProInvest.Services.Backend
{
    public interface IUsuarios
    {
        public Task<List<string>> GetEstadosAsync();
        public Task<List<InversionistaViewModel>> GetColoniasPorCodigoPostalAsync(string direccionIp, string codigoPostal);
        public Task<InversionistaViewModel> AnadirInformacionPersonalInversionistaAsync(InversionistaViewModel datosPersonales);
        public Task<ContratoInversionRespuestaJson> CrearContratoInversionAsync(string ip, int id, DateTime fechaActualizacion);
        public Task<ContratoInversionRespuestaJson> ObtenerContratoInversionPorIpAsync(string ip);
        public Task<bool> EditarEstadoUltimaActualizacionContratoInversionAsync(int idInversionista, string nuevoEstado, DateTime fechaActualizacion, string token);
        public Task<bool> AgregarVerificacionesCorreo(int idInversionista);
        public Task<bool> EnviarCorreoVerificacion(int idInversionista, int folioContrato, string token);
        public Task<ContratoInversionRespuestaJson> ObtenerContratoPorFolioInversion(int folioSolicitado);
        public Task<InversionistaViewModel> AnadirInformacionDomicilioInversionistaAsync(InversionistaViewModel direccion, string token);
        public Task<bool> CrearInformacionBancaria(InformacionBancariaViewModel datosAIngresar, int folioInversion, string token);
        public Task<ContratoInversionModel> EditarInversionContratoInversion(InformacionBancariaViewModel datosAIngresar, int idInversionista, string token);
        public Task<ContratoInversionModel> AgregarContratoCompletoContratoInversionAsync(string base64Url, int idInversionista, string token);
        public Task<IEnumerable<TipoInversionViewModel>> ObtenerTiposInversionAsync();
        public Task<bool> SubirContratoInversion(ExpedienteInversionistaViewModel expedienteInversionista, string token);
    }
}
