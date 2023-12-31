using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.Text;
using System.Text.Json;

namespace Frontend_ProInvest.Services.Backend{
    public class Administrador : IAdministrador{
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Administrador(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CredencialesRespuestaJson> ObtenerCredencialesAccesoAsync(CredencialesAccesoViewModel credencialesAcceso)
        {
            CredencialesRespuestaJson credencialesObtenidas = new();
            var requestData = new
            {
                usuario = credencialesAcceso.Usuario,
                contrasena = credencialesAcceso.Contrasena
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIAdministrador"]}/login")
            {
                Content = jsonContent
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    credencialesObtenidas = await response.Content.ReadFromJsonAsync<CredencialesRespuestaJson>();
                    credencialesObtenidas.CodigoStatus = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                credencialesObtenidas.Usuario = null;
                credencialesObtenidas.CodigoStatus = System.Net.HttpStatusCode.InternalServerError;
            }
            return credencialesObtenidas;
        }   
    }
}