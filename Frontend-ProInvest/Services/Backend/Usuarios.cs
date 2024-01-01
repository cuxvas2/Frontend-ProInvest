using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.Text;
using System.Text.Json;

namespace Frontend_ProInvest.Services.Backend
{
    public class Usuarios : IUsuarios
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public Usuarios(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<string>> GetEstadosAsync()
        {
            List<string> estados = new();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIInversionista"]}/estados");
            
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    estados = await response.Content.ReadFromJsonAsync<List<string>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los estados");
            }
            return estados;
        } 
        public async Task<List<DireccionViewModel>> GetColoniasPorCodigoPostalAsync(string direccionIp, string codigoPostal)
        {
            List<DireccionViewModel> colonias = new();            
            var requestData = new
            {
                ip = direccionIp
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/colonia/{codigoPostal}")
            {
                Content = jsonContent
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    DireccionRespuestaJson respuesta = await response.Content.ReadFromJsonAsync<DireccionRespuestaJson>();
                    foreach(var colonia in respuesta.Colonias)
                    {
                        colonias.Add(colonia);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron recuperar las colonias");
            }
            return colonias;
        }
        public async Task AnadirInformacionPersonalInversionista (InversionistaViewModel datosPersonales)
        {
            InversionistaViewModel inversionistaRetornado;
            string token;
            StringContent jsonContent = new(JsonSerializer.Serialize(datosPersonales), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/anadirInformacionPersonalInversionista")
            {
                Content = jsonContent
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    DireccionRespuestaJson respuesta = await response.Content.ReadFromJsonAsync<DireccionRespuestaJson>();
                    foreach (var colonia in respuesta.Colonias)
                    {
                        colonias.Add(colonia);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron recuperar las colonias");
            }
        }
    }
}
