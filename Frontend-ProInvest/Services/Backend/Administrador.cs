using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Diagnostics.Contracts;

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

        public async Task<IEnumerable<TipoInversionViewModel>> GetTiposInversionAsync(string accessToken)
        {
            List<TipoInversionViewModel> usuarios = new();
            IEnumerable<TipoInversionViewModel> tipoInversiones = usuarios;

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/tiposInversion")
            {
                Headers = { { "token", accessToken } }
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
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    nombre = inversion.Nombre,
                    descripcion = inversion.Descripcion,
                    rendimiento = inversion.Rendimiento
                }),
                Encoding.UTF8,
                "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIAdministrador"]}/tiposInversion")
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

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIAdministrador"]}/tiposInversion/{inversion.IdTipo}")
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

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["UrlWebAPIAdministrador"]}/tiposInversion/{id}")
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
        public async Task<OrigenInversionRespuestaJson> ObtenerOrigenesInversion(string token)
        {
            OrigenInversionRespuestaJson origenesInversion = new();
            
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/origenesInversion")
            {
                Headers = { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    origenesInversion = await response.Content.ReadFromJsonAsync<OrigenInversionRespuestaJson>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los origenes de inversi�n");
            }
            return origenesInversion;
        }

        public async Task<IEnumerable<InformacionContrato>> ObtenerContratos(string token)
        {
            List<InformacionContrato> contratos = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/contratosInversion")
            {
                Headers = { { "token", token } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    contratos = await response.Content.ReadFromJsonAsync<List<InformacionContrato>>();
                }
            }
            catch (Exception ex)
            {
            }
            IEnumerable<InformacionContrato> contratosObtenidos = contratos;
            return contratosObtenidos;
        }

        public async Task<SolicitudInversionViewModel> ObtenerSolicitudInversion(string token, InformacionContrato contrato)
        {
            SolicitudInversionViewModel solicitud = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/inversionistas")
            {
                Headers = { { "token", token } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                solicitud.CodigoSolicitud = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    List<InversionistaViewModel> Listainversionista = new();
                    Listainversionista = await response.Content.ReadFromJsonAsync<List<InversionistaViewModel>>();
                    solicitud.Inversionista = Listainversionista.FirstOrDefault(x => x.IdInversionista == contrato.IdInversionista);
                }
                solicitud.InformacionBancaria = await ObtenerInformacionBancariaConFolioInversion(token, contrato.FolioInversion);
                solicitud.Documentos = await ObtenerNombresDocumentosExpediente(token);
                if(solicitud.Documentos != null)
                {
                    foreach(DocumentosExpediente doc in solicitud.Documentos)
                    {
                        try
                        {
                            doc.URLDoc = await ObtenerURLDeDocuemnto(token, doc.IdDocumento, contrato.IdInversionista);

                        }
                        catch(Exception ex)
                        {

                        }
                    }
                    //Teminar de obtener los origenes

                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudieron recuperar los bancos");
            }

            return solicitud;
        }

        private async Task<URLDocumento> ObtenerURLDeDocuemnto (string token, int idDocumento, int idInversionista)
        {
            URLDocumento documento = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/expedientesInversionistas/{idDocumento}/{idInversionista}")
            {
                Headers = { { "token", token } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    documento = await response.Content.ReadFromJsonAsync<URLDocumento>();
                }
            }
            catch (Exception ex)
            {

            }

            return documento;
        }

        private async Task<List<DocumentosExpediente>> ObtenerNombresDocumentosExpediente (string token)
        {
            List<DocumentosExpediente> solicitudes = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/documentosExpediente")
            {
                Headers = { { "token", token } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    solicitudes = await response.Content.ReadFromJsonAsync<List<DocumentosExpediente>>();
                }
            }
            catch (Exception ex)
            {

            }

            return solicitudes;
        }

        private async Task<InformacionBancariaViewModel> ObtenerInformacionBancariaConFolioInversion(string token, int folioInversion)
        {
            InformacionBancariaViewModel solicitud = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/informacionBancaria/{folioInversion}")
            {
                Headers = { { "token", token } }
            };

            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    solicitud = await response.Content.ReadFromJsonAsync<InformacionBancariaViewModel>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudieron recuperar los bancos");
            }

            return solicitud;
        }

        public async Task<InformacionContrato> ObtenerInformacionContratoPorFolio(string token, int folio)
        {
            InformacionContrato contrato = new();
            var listaTiposInversion = await ObtenerContratos(token);
            contrato = listaTiposInversion.FirstOrDefault(x => x.FolioInversion == folio);
            return contrato;
        }
        public async Task<IEnumerable<DocumentosExpedienteViewModel>> ObtenerDocumentosExpediente(string token)
        {
            List<DocumentosExpedienteViewModel> documentos = new();
            
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIAdministrador"]}/documentosExpediente")
            {
                Headers =  { { "token", token} }
            };
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    documentos = await response.Content.ReadFromJsonAsync<List<DocumentosExpedienteViewModel>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("No se pudieron recuperar los documentos");
            }
            IEnumerable<DocumentosExpedienteViewModel> documentosObtenidos = documentos;
            return documentosObtenidos;
        }
        public async Task<HttpStatusCode> RegistrarOrigenInversion(string origenInversion, string token)
        {
            var requestData = new
            {
                nombre = origenInversion
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIAdministrador"]}/origenesInversion")
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
                throw new Exception("No se pudieron ingresar los origenes de inversion");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EditarOrigenInversion(OrigenInversionViewModel origenEditado, string token)
        {
            var requestData = new
            {
                nombre = origenEditado.NombreOrigen
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIAdministrador"]}/origenesInversion/{origenEditado.IdOrigen}")
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
                throw new Exception("No se pudo editar el origen de inversion");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EliminarOrigenInversion(int idOrigenInversion, string token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["UrlWebAPIAdministrador"]}/origenesInversion/{idOrigenInversion}")
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
                throw new Exception("No se pudo eliminar el origen de inversion");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> RegistrarDocumento(string documentoExpediente, string token)
        {
            var requestData = new
            {
                nombre = documentoExpediente
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIAdministrador"]}/documentosExpediente")
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
                throw new Exception("No se pudieron ingresar los documentos para expediente");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EditarDocumento(DocumentosExpedienteViewModel documentoExpediente, string token)
        {
            var requestData = new
            {
                nombre = documentoExpediente.NombreDocumento
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIAdministrador"]}/documentosExpediente/{documentoExpediente.IdDocumento}")
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
                throw new Exception("No se pudo editar el documento para expediente");
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<HttpStatusCode> EliminarDocumento(int idDocumento, string token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["UrlWebAPIAdministrador"]}/documentosExpediente/{idDocumento}")
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
                throw new Exception("No se pudo eliminar el documento para expediente");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}