using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend
{
    public interface IAdministrador
    {
        public Task<IEnumerable<TipoInversionViewModel>> GetTiposInversionAsync(string accessToken);
        public Task<bool> PostTiposInversionAsync(string accessToken, TipoInversionViewModel inversion);
    }
}
