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
        public async Task<List<InversionistaViewModel>> GetColoniasPorCodigoPostalAsync(string direccionIp, string codigoPostal)
        {
            List<InversionistaViewModel> colonias = new();            
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
                    InversionistaRespuestaJson respuesta = await response.Content.ReadFromJsonAsync<InversionistaRespuestaJson>();
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
        public async Task<InversionistaViewModel> AnadirInformacionPersonalInversionistaAsync (InversionistaViewModel datosPersonales)
        {
            InversionistaViewModel inversionistaRetornado = new();
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{_configuration["UrlWebAPIInversionista"]}/informacionPersonal", datosPersonales);
                if (response.IsSuccessStatusCode)
                {
                    InversionistaRespuestaJson respuesta = await response.Content.ReadFromJsonAsync<InversionistaRespuestaJson>();
                    inversionistaRetornado = respuesta.Inversionista;
                    inversionistaRetornado.Token = respuesta.Token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron guardar los datos");
            }
            return inversionistaRetornado;
        }
        public async Task<bool> CrearContratoInversionAsync(string ip, int id, DateTime fechaActualizacion)
        {
            bool contratoCreado = false;
            string fechaFormateada = fechaActualizacion.ToString("yyyy-MM-dd HH:mm:ss");

            var requestData = new
            {
                direccionIp = ip,
                idInversionista = id,
                ultimaActualizacion = fechaFormateada
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{_configuration["UrlWebAPIInversionista"]}/contratosInversion", requestData);
                if (response.IsSuccessStatusCode)
                {
                    contratoCreado = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudo crear el contrato de inversión");
            }
            return contratoCreado;
        }
        public async Task<ContratoInversionRespuestaJson> ObtenerContratoInversionPorIpAsync (string ip)
        {
            ContratoInversionRespuestaJson respuesta = new();
            var requestData = new
            {
                direccionIp = ip
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/obtenerIp")
            {
                Content = jsonContent
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    respuesta = await response.Content.ReadFromJsonAsync<ContratoInversionRespuestaJson>();                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudo recuperar el contrato");
            }
            return respuesta;
        }
    }
}
