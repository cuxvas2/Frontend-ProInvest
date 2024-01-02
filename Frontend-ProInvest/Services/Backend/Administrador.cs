using Frontend_ProInvest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
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


        public async Task<IEnumerable<TipoInversionViewModel>> GetTiposInversionAsync(string accessToken)
        {
            List<TipoInversionViewModel> usuarios = new();
            IEnumerable<TipoInversionViewModel> tipoInversiones = usuarios;

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPI"]}/admin/tiposInversion")
            {
                Headers = { { "token", accessToken} }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    usuarios = await response.Content.ReadFromJsonAsync<List<TipoInversionViewModel>>();
                    tipoInversiones = usuarios;
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caído
            }

            return tipoInversiones;
        }

        public async Task<bool> AnadirTiposInversionAsync(string accessToken, TipoInversionViewModel inversion)
        {
            bool exitoso = false;
            // Para enviar al usuario, lo convierto a JSON
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    nombre = inversion.Nombre,
                    descripcion = inversion.Descripcion,
                    rendimiento = inversion.Rendimiento
                }),
                Encoding.UTF8,
                "application/json");
            // Preparo la llamada con el JSON con los datos del login
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPI"]}/admin/tiposInversion")
            {
                Content = jsonContent,
                Headers = { { "token", accessToken } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                // Realizo la llamada al Web API
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    //Regresa un TipoInversion
                    //token = await response.Content.ReadFromJsonAsync<AuthUser>();
                    exitoso = true;
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caído
            }

            return exitoso;
        }

        public async Task<TipoInversionViewModel> GetTipoInversionAsync(string token, int id)
        {
            TipoInversionViewModel tipoInversion = new();
            var listaTiposInversion = await GetTiposInversionAsync(token);
            tipoInversion = listaTiposInversion.FirstOrDefault(x => x.IdTipo == id);
            return tipoInversion;
        }

        public async Task<bool> EditarTipoInversionAsync(string accessToken, TipoInversionViewModel inversion)
        {
            bool exitoso = false;

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    nombre = inversion.Nombre,
                    descripcion = inversion.Descripcion,
                    rendimiento = inversion.Rendimiento
                }),
                Encoding.UTF8,
                "application/json");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPI"]}/admin/tiposInversion/{inversion.IdTipo}")
            {
                Content = jsonContent,
                Headers = { { "token", accessToken } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    exitoso = true;
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caído
            }

            return exitoso;
        }

        public async Task<bool> EliminarTipoInversionAsync(string accessToken, int id)
        {
            bool eliminacionExitosa = false;

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["UrlWebAPI"]}/admin/tiposInversion/{id}")
            {
                Headers = { { "token", accessToken } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    eliminacionExitosa = true;
                }
            }
            catch (Exception)
            {
                // No se pudo conectar porque el servicio esta caído
            }

            return eliminacionExitosa;
        }
    }
}
