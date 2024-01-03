using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Frontend_ProInvest.Services.Backend
{
    public class Administrador : IAdministrador
    {
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
                return credencialesObtenidas;
            }
            return credencialesObtenidas;
        }
        public async Task<IEnumerable<BancosViewModel>> ObtenerBancos(string token)
        {
            List<BancosViewModel> bancos = new();
            
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/bancos")
            {
                Headers =  { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    bancos = await response.Content.ReadFromJsonAsync<List<BancosViewModel>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los bancos");
            }
            IEnumerable<BancosViewModel> bancosObtenidos = bancos;
            return bancosObtenidos;
        }
        public async Task<HttpStatusCode> RegistrarBanco(string nombreBanco, string token)
        {
            var requestData = new
            {
                nombre = nombreBanco
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIAdministrador"]}/bancos")
            {
                Content = jsonContent,
                Headers =  { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los bancos");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EditarBanco(BancosViewModel bancoNuevo, string token)
        {
            var requestData = new
            {
                nombre = bancoNuevo.Banco
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIAdministrador"]}/bancos/{bancoNuevo.IdBanco}")
            {
                Content = jsonContent,
                Headers =  { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los bancos");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EliminarBanco(int idBanco, string token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["UrlWebAPIAdministrador"]}/bancos/{idBanco}")
            {
                Headers =  { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los bancos");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}