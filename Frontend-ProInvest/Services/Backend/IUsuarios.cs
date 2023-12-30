using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend
{
    public interface IUsuarios
    {
        public Task<List<string>> GetEstadosAsync();
        public Task<List<DireccionViewModel>> GetColoniasPorCodigoPostalAsync(string direccionIp, string codigoPostal);
    }
}
