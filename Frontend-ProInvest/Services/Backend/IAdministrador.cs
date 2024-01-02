using Frontend_ProInvest.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend_ProInvest.Services.Backend
{
    public interface IAdministrador
    {
        public Task<IEnumerable<TipoInversionViewModel>> GetTiposInversionAsync(string accessToken);
        public Task<bool> AnadirTiposInversionAsync(string accessToken, TipoInversionViewModel inversion);
        public Task<TipoInversionViewModel> GetTipoInversionAsync(string token, int id);
        public Task<bool> EditarTipoInversionAsync(string token, TipoInversionViewModel inversion);
        public Task<bool> EliminarTipoInversionAsync(string token, int id);
    }
}
