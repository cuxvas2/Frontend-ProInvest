using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using NuGet.Common;
using System.Net.Http.Headers;
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
        public async Task<ContratoInversionRespuestaJson> CrearContratoInversionAsync(string ip, int id, DateTime fechaActualizacion)
        {
            ContratoInversionRespuestaJson contratoCreado = new();
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
                    var informacionContrato = await response.Content.ReadFromJsonAsync<ContratoInversionModel>();
                    contratoCreado.InformacionContrato = informacionContrato;
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
        public async Task<bool> EditarEstadoUltimaActualizacionContratoInversionAsync(int idInversionista, string nuevoEstado, DateTime fechaActualizacion, string token)
        {
            bool estadoActualizado = false;
            string fechaFormateada = fechaActualizacion.ToString("yyyy-MM-dd HH:mm:ss");

            var requestData = new
            {
                estado = nuevoEstado,
                ultimaActualizacion = fechaFormateada
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/estado/{idInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    estadoActualizado = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudo actualizar el estado del contrato de inversión");
            }
            return estadoActualizado;
        }
        public async Task<bool> AgregarVerificacionesCorreo(int idInversionista)
        {
            bool verificacionCorrecta = false;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/correo/{idInversionista}");
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if(response.IsSuccessStatusCode)
                {
                    verificacionCorrecta = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se verificar la dirección de correo electrónico, intente de nuevo más tarde.");
            }
            return verificacionCorrecta;
        }
        public async Task<bool> EnviarCorreoVerificacion (int idInversionista, int folioContrato, string token)
        {
            bool envioCorrecto = false;
            var requestData = new
            {
                folioInversion = folioContrato
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/enviarCorreo/{idInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    envioCorrecto = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("Ocurrió un error al enviar el correo de verificación. Intente de nuevo.");
            }
            return envioCorrecto;
        }
        public async Task<ContratoInversionRespuestaJson> ObtenerContratoPorFolioInversion (int folioSolicitado)
        {
            ContratoInversionRespuestaJson respuesta = new();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/{folioSolicitado}");
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var informacionContrato = await response.Content.ReadFromJsonAsync<ContratoInversionModel>();
                    respuesta.InformacionContrato = informacionContrato;
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
        public async Task<InversionistaViewModel> AnadirInformacionDomicilioInversionistaAsync(InversionistaViewModel direccion, string token)
        {
            InversionistaViewModel inversionistaRetornado = new();
            var requestData = new
            {
                calle = direccion.Calle,
                colonia = direccion.Colonia,
                codigoPostal = direccion.CodigoPostal,
                estado = direccion.Estado,
                municipio = direccion.Municipio,
                numeroExterior = direccion.NumeroExterior,
                numeroInterior = direccion.NumeroInterior,
                direccionIp = direccion.DireccionIp
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIInversionista"]}/informacionDomicilio/{direccion.IdInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    InversionistaRespuestaJson respuesta = await response.Content.ReadFromJsonAsync<InversionistaRespuestaJson>();
                    if(respuesta.InversionistaNuevo.Length  > 1) 
                    {
                        inversionistaRetornado = respuesta.InversionistaNuevo[1];
                    }
                    inversionistaRetornado.Token = respuesta.Token;
                }
                else
                {
                    throw new Exception();
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
        public async Task<bool> CrearInformacionBancaria(InformacionBancariaViewModel datosAIngresar, int folioInversion, string token)
        {
            bool informacionBancariaCreada = false;
            var requestData = new
            {
                cuenta = datosAIngresar.Cuenta,
                clabeInterbancaria = datosAIngresar.ClabeInterbancaria,
                idBanco = datosAIngresar.IdBanco
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/informacionBancaria/{folioInversion}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    informacionBancariaCreada = true;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron guardar los datos");
            }
            return informacionBancariaCreada;
        }
        public async Task<ContratoInversionModel>EditarInversionContratoInversion(InformacionBancariaViewModel datosAIngresar, int idInversionista, string token)
        {
            ContratoInversionModel contrato = new();
            var requestData = new
            {
                idTipo = datosAIngresar.IdTipo,
                idOrigen = datosAIngresar.IdOrigen,
                importe = datosAIngresar.CantidadAInvertir,
                plazoAnios = datosAIngresar.Anios
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/{idInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if(response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<ContratoInversionRespuestaJson>();
                    if(respuesta.ContratoActualizado.Count > 1)
                    {
                        contrato = respuesta.ContratoActualizado[1];
                    }
                }
                else
                {
                    throw new Exception();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron guardar los datos");
            }
            return contrato;
        }
        public async Task<ContratoInversionModel> AgregarContratoCompletoContratoInversionAsync(string base64Url, int idInversionista, string token)
        {
            ContratoInversionModel contrato = new();
            var requestData = new
            {
                contrato = base64Url
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, $"{_configuration["UrlWebAPIInversionista"]}/contratosInversion/contrato/{idInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if(response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<ContratoInversionRespuestaJson>();
                    if(respuesta.ContratoActualizado.Count > 1)
                    {
                        contrato = respuesta.ContratoActualizado[1];
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudieron guardar los cambios");
            }
            return contrato;
        }
        public async Task<IEnumerable<TipoInversionViewModel>> ObtenerTiposInversionAsync()
        {
            List<TipoInversionViewModel> tiposAux = new();
            IEnumerable<TipoInversionViewModel> tipoInversiones = tiposAux;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["UrlWebAPIInversionista"]}/tiposInversion");
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    tiposAux = await response.Content.ReadFromJsonAsync<List<TipoInversionViewModel>>();
                    tipoInversiones = tiposAux;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("Ocurrió un error al recuperar los tipos de inversión, intente de nuevo más tarde.");
            }

            return tipoInversiones;
        }
        public async Task<bool> SubirContratoInversion(ExpedienteInversionistaViewModel expedienteInversionista, string token)
        {
            bool contratoSubido = false;
            var idInversionista = expedienteInversionista.IdInversionista;
            var idDocumento = expedienteInversionista.IdDocumento;
            var requestData = new
            {
                nombreArchivo = expedienteInversionista.NombreDocumento,
                enlaceBucket = expedienteInversionista.EnlaceBucket
            };
            StringContent jsonContent = new(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["UrlWebAPIInversionista"]}/expedientesInversionistas/{idDocumento}/{idInversionista}")
            {
                Content = jsonContent
            };
            httpRequestMessage.Headers.Add("token", token);
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    contratoSubido = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new Exception("No se pudo agregar su expediente de inversión");
            }
            return contratoSubido;
        }
    }
}
