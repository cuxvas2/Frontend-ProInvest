using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Frontend_ProInvest.Controllers
{
    public class FormularioController : Controller
    {
        private readonly IUsuarios _usuarios;
        private readonly IAdministrador _administrador;
        private readonly IAmazonS3 _amazons3;
        public FormularioController(IUsuarios usuarios, IAdministrador administrador, IAmazonS3 amazons3)
        {
            _usuarios = usuarios;
            _administrador = administrador;
            _amazons3 = amazons3;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DatosPersonales()
        {
            try
            {
                var direccionIp = ObtenerDireccionIp();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                if (solicitudExistente?.InformacionContrato != null)
                {
                    string estado = solicitudExistente.InformacionContrato.Estado;
                    switch (estado)
                    {
                        case "VERIFICACION":
                            return RedirectToAction("VerificacionDatosContacto");
                        case "DOMICILIO":
                            return RedirectToAction("Direccion");
                        case "FINANCIERO":
                            return RedirectToAction("InformacionBancaria");
                        case "EXPEDIENTE":
                            return RedirectToAction("Expediente");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo recuperar el proceso de su solicitud. Intente de nuevo más tarde";
            }
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(NivelEstudios));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatosPersonalesAsync(InversionistaViewModel datosPersonales,
            string BtnPrevious, string BtnNext)
        {
            var direccionIp = ObtenerDireccionIp();
            datosPersonales.DireccionIp = direccionIp;
            if (BtnNext != null)
            {
                try
                {
                    var resultado = await _usuarios.AnadirInformacionPersonalInversionistaAsync(datosPersonales);
                    var contrato = await _usuarios.CrearContratoInversionAsync(direccionIp, resultado.IdInversionista, DateTime.UtcNow);
                    if (resultado?.Token != null && contrato.InformacionContrato != null)
                    {
                        var correoEnviado = await _usuarios.EnviarCorreoVerificacion(resultado.IdInversionista, (int)contrato.InformacionContrato.FolioInversion, resultado.Token);
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Path = "/Formulario",
                        };
                        Response.Cookies.Append("Token", resultado.Token, cookieOptions);
                        return RedirectToAction("VerificacionDatosContacto");
                    }
                    else
                    {
                        ViewBag.Error = "Ocurrió un error al guarda la información, intente de nuevo";
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Ocurrió un error al guarda la información, intente de nuevo";
                }
            }
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(NivelEstudios));
            return View("DatosPersonales");
        }

        public async Task<IActionResult> VerificacionDatosContacto()
        {
            try
            {
                var direccionIp = ObtenerDireccionIp();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                if (solicitudExistente?.InformacionContrato != null)
                {
                    string estado = solicitudExistente.InformacionContrato.Estado;
                    switch (estado)
                    {
                        case "DOMICILIO":
                            return RedirectToAction("Direccion");
                        case "FINANCIERO":
                            return RedirectToAction("InformacionBancaria");
                        case "EXPEDIENTE":
                            return RedirectToAction("Expediente");
                            break;
                    }
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/Formulario",
                    };
                    Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                    Response.Cookies.Append("IdInversionista", solicitudExistente.InformacionContrato.IdInversionista.ToString(), cookieOptions);
                    Response.Cookies.Append("FolioContrato", solicitudExistente.InformacionContrato.FolioInversion.ToString(), cookieOptions);
                    if (solicitudExistente.InformacionContrato.CorreoVerificacion == true)
                    {
                        ViewBag.CorreoVerificacion = true;
                        var estadoActualizado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync((int)solicitudExistente.InformacionContrato.IdInversionista, "DOMICILIO", DateTime.UtcNow, solicitudExistente.Token);
                        if (estadoActualizado)
                        {
                            return RedirectToAction("Direccion");
                        }
                        else
                        {
                            ViewBag.Error = "Ocurrió un error al procesar tu solicitud, intente de nuevo";
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo recuperar el proceso de su solicitud. Intente de nuevo más tarde";
                return RedirectToAction("DatosPersonales");
            }
            return View("VerificacionDatosContacto");
        }
        [HttpPost]
        public async Task<IActionResult> CorreoVerificado(string BtnContinuar)
        {
            if (BtnContinuar != null)
            {
                try
                {
                    var direccionIp = ObtenerDireccionIp();
                    var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                    if (solicitudExistente?.InformacionContrato != null)
                    {
                        if (solicitudExistente.InformacionContrato.CorreoVerificacion == true)
                        {
                            var estadoActualizado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync((int)solicitudExistente.InformacionContrato.IdInversionista, "DOMICILIO", DateTime.UtcNow, solicitudExistente.Token);
                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None,
                                Path = "/Formulario",
                            };
                            Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                            return RedirectToAction("Direccion");
                        }
                        else
                        {
                            ViewBag.Error = "Debes verificar tu correo electrónico para continuar";
                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Ocurrió un error al verificar tu cuenta, por favor intenta de nuevo.";
                }
            }
            return View("VerificacionDatosContacto");
        }
        [HttpPost]
        public async Task<ActionResult> EnviarCorreo(string BtnSendEmail)
        {
            if (BtnSendEmail != null)
            {
                int idInversionista = Int32.Parse(Request.Cookies["IdInversionista"]);
                string token = Request.Cookies["Token"];
                int folioContrato = Int32.Parse(Request.Cookies["FolioContrato"]);
                try
                {
                    var correoEnviado = await _usuarios.EnviarCorreoVerificacion(idInversionista, folioContrato, token);
                    if (correoEnviado)
                    {
                        ViewBag.Exito = "Se ha enviado la liga para verificar la dirección de correo electrónico. Revisa tu bandeja de entrada y en la carpeta No deseados";
                    }
                    else
                    {
                        throw new Exception("Ocurrió un error al enviar el correo electrónico. Intente nuevamente.");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }
            return View("VerificacionDatosContacto");
        }
        public async Task<IActionResult> Direccion()
        {
            try
            {
                var direccionIp = ObtenerDireccionIp();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                if (solicitudExistente?.InformacionContrato != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/Formulario",
                    };
                    Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                    Response.Cookies.Append("IdInversionista", solicitudExistente.InformacionContrato.IdInversionista.ToString(), cookieOptions);
                    string estado = solicitudExistente.InformacionContrato.Estado;
                    switch (estado)
                    {
                        case "VERIFICACION":
                            return RedirectToAction("VerificacionDatosContacto");
                        case "FINANCIERO":
                            return RedirectToAction("InformacionBancaria");
                        case "EXPEDIENTE":
                            return RedirectToAction("Expediente");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo recuperar el proceso de su solicitud. Intente de nuevo más tarde";
                return RedirectToAction("DatosPersonales");
            }
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(NivelEstudios));
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Direccion(InversionistaViewModel direccion, string BtnPrevious, string BtnNext)
        {
            if (BtnNext != null)
            {
                try
                {
                    direccion.DireccionIp = ObtenerDireccionIp();
                    int idInversionista = Int32.Parse(Request.Cookies["IdInversionista"]);
                    string token = Request.Cookies["Token"];
                    direccion.IdInversionista = idInversionista;
                    var inversionistaActualizado = await _usuarios.AnadirInformacionDomicilioInversionistaAsync(direccion, token);
                    var estadoCambiado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(idInversionista, "FINANCIERO", DateTime.UtcNow, token);
                    if (estadoCambiado)
                    {
                        return RedirectToAction("InformacionBancaria");
                    }
                    else
                    {
                        ViewBag.Error = "No se pudo guardar el proceso de tu solicitud. Intente de nuevo más tarde";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }
            return View();
        }
        public async Task<IActionResult> Expediente()
        {
            try
            {
                var direccionIp = ObtenerDireccionIp();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                if (solicitudExistente?.InformacionContrato != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/Formulario",
                    };
                    Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                    Response.Cookies.Append("IdInversionista", solicitudExistente.InformacionContrato.IdInversionista.ToString(), cookieOptions);
                    string estado = solicitudExistente.InformacionContrato.Estado;
                    switch (estado)
                    {
                        case "VERIFICACION":
                            return RedirectToAction("VerificacionDatosContacto");
                        case "DOMICILIO":
                            return RedirectToAction("Direccion");
                        case "FINANCIERO":
                            return RedirectToAction("InformacionBancaria");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo recuperar el proceso de su solicitud. Intente de nuevo más tarde";
                return RedirectToAction("DatosPersonales");
            }
            string token = Request.Cookies["Token"];
            var listaDocumentos = await _administrador.ObtenerDocumentosExpediente(token);
            return View(listaDocumentos);
        }
        [HttpPost]
        public async Task<IActionResult> Expediente(List<IFormFile> archivos, List<string> nombresArchivos, List<int> documentoId)
        {
            bool todosSubidos = true;
            int idInversionista = Int32.Parse(Request.Cookies["IdInversionista"]);
            string token = Request.Cookies["Token"];
            if (archivos.Count != nombresArchivos.Count || archivos.Count != documentoId.Count)
            {
                ViewBag.Error = "Debe subir todos los archivos";
                var listaDocumentosObtenidos = await _administrador.ObtenerDocumentosExpediente(token);
                return View(listaDocumentosObtenidos);
            }
            if (archivos.Any(f => Path.GetExtension(f.FileName).ToLower() != ".pdf"))
            {
                ViewBag.Error = "Todos los archivos tienen que ser PDF";
                var listaDocumentosObtenidos = await _administrador.ObtenerDocumentosExpediente(token);
                return View(listaDocumentosObtenidos);
            }
            if (archivos.Any(f => f.Length > 5 * 1024 * 1024))
            {
                ViewBag.Error = "Todos los archivos deben ser menores de 5MB";
                var listaDocumentosObtenidos = await _administrador.ObtenerDocumentosExpediente(token);
                return View(listaDocumentosObtenidos);
            }
            for (int i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                var nombreArchivo = nombresArchivos[i];
                var idDocumento = documentoId[i];
                var subido = await _amazons3.SubirArchivo(nombreArchivo, archivo);
                if (subido)
                {
                    var url = _amazons3.ObtenerUrlArchivo(nombreArchivo);
                    var expediente = new ExpedienteInversionistaViewModel
                    {
                        IdInversionista = idInversionista,
                        EnlaceBucket = url,
                        IdDocumento = idDocumento,
                        NombreDocumento = nombreArchivo
                    };
                    var expedienteSubido = await _usuarios.SubirContratoInversion(expediente, token);
                    if (!expedienteSubido)
                    {
                        ViewBag.Error = "Ocurrió un error al guardar los archivos en la base de datos";
                        todosSubidos = false;
                        continue;
                    }

                }
                else
                {
                    ViewBag.Error = "Ocurrió un error al subir los archivos";
                    todosSubidos = false;
                    continue;
                }
            }
            if (todosSubidos)
            {
                var estadoCambiado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(idInversionista, "FINALIZADO", DateTime.UtcNow, token);
                if (!estadoCambiado)
                {
                    ViewBag.Error = "No se pudo guardar el proceso de tu solicitud. Intente de nuevo más tarde";
                }
                else
                {
                    ViewBag.Error = "Subido correctamente";
                }
            }
            var listaDocumentos = await _administrador.ObtenerDocumentosExpediente(token);
            return View(listaDocumentos);

        }
        public async Task<ActionResult> InformacionBancaria()
        {
            try
            {
                var direccionIp = ObtenerDireccionIp();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                if (solicitudExistente?.InformacionContrato != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/Formulario",
                    };
                    Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                    Console.WriteLine("\nToken generado en contrato: " + solicitudExistente.Token);
                    Response.Cookies.Append("FolioInversion", solicitudExistente.InformacionContrato.FolioInversion.ToString(), cookieOptions);
                    Response.Cookies.Append("IdInversionista", solicitudExistente.InformacionContrato.IdInversionista.ToString(), cookieOptions);
                    string estado = solicitudExistente.InformacionContrato.Estado;
                    switch (estado)
                    {
                        case "VERIFICACION":
                            return RedirectToAction("VerificacionDatosContacto");
                        case "DOMICILIO":
                            return RedirectToAction("Direccion");
                        case "EXPEDIENTE":
                            return RedirectToAction("Expediente");
                            break;
                    }
                }
                var token = Request.Cookies["Token"];
                Console.WriteLine("\nToken recuperado de cookie contrato: " + token);
                var origenesToken = await _administrador.ObtenerOrigenesInversion(token);
                if (origenesToken.Token != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/Formulario",
                    };
                    Response.Cookies.Append("Token", solicitudExistente.Token, cookieOptions);
                    Console.WriteLine("\nToken creado en origenes: " + solicitudExistente.Token);
                    if (origenesToken?.OrigenesInversion != null)
                    {
                        string origenesJson = JsonConvert.SerializeObject(origenesToken.OrigenesInversion);
                        TempData["OrigenesInversion"] = origenesJson;
                        SelectList selectList = new(origenesToken.OrigenesInversion, "IdOrigen", "NombreOrigen");
                        ViewBag.OrigenesInversion = selectList;
                    }
                    else
                    {
                        ViewBag.Error = "No se pudieron recuperar los origenes de inversión";
                    }
                }
                token = Request.Cookies["Token"];
                Console.WriteLine("\nToken recuperado cookie origen: " + token);
                var bancos = await _administrador.ObtenerBancos(token);
                if (bancos?.Count() > 0)
                {
                    string bancosJson = JsonConvert.SerializeObject(bancos);
                    TempData["Bancos"] = bancosJson;
                    SelectList selectList = new(bancos, "IdBanco", "Banco");
                    ViewBag.Bancos = selectList;
                }
                else
                {
                    ViewBag.Error = "No se pudieron recuperar los bancos";
                }
                Console.WriteLine("\n\n*********************************************\nToken recuperado cookie banco: " + token);
                var tiposInversion = await _administrador.GetTiposInversionAsync(token);
                if (tiposInversion?.Count() > 0)
                {
                    string tiposInversionJson = JsonConvert.SerializeObject(tiposInversion);
                    TempData["TiposInversion"] = tiposInversionJson;
                    SelectList selectList = new(tiposInversion, "IdTipo", "Nombre");
                    ViewBag.TiposInversion = selectList;
                }
                else
                {
                    ViewBag.Error = "No se pudieron recuperar los tipos de inversión";
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo recuperar el proceso de su solicitud. Intente de nuevo más tarde";
                return RedirectToAction("DatosPersonales");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> InformacionBancaria(InformacionBancariaViewModel modelo, string BtnPrevious, string BtnNext)
        {
            if (BtnNext != null)
            {
                if (ModelState.IsValid)
                {
                    if (modelo.OrigenLicito == false)
                    {
                        ModelState.AddModelError("OrigenLicito", "Debe aceptar el Acuerdo de Origen de Fondos para continuar.");
                    }
                    if (modelo.AceptaContrato == false)
                    {
                        ModelState.AddModelError("AceptaContrato", "Debe aceptar el Contrato de inversión para continuar.");
                    }
                }
                if (!ModelState.IsValid)
                {
                    string token = Request.Cookies["Token"];
                    int folioInversion = Int32.Parse(Request.Cookies["FolioInversion"]);
                    int idInversionista = Int32.Parse(Request.Cookies["IdInversionista"]);
                    try
                    {
                        modelo.IdBanco = Int32.Parse(modelo.Banco);
                        modelo.IdTipo = Int32.Parse(modelo.TipoDeInversion);
                        modelo.IdOrigen = Int32.Parse(modelo.OrigenDeFondos);

                        var informacionBancaria = await _usuarios.CrearInformacionBancaria(modelo, folioInversion, token);
                        var contratoActualizado = await _usuarios.EditarInversionContratoInversion(modelo, idInversionista, token);
                        if (informacionBancaria && contratoActualizado != null)
                        {
                            var estadoCambiado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(idInversionista, "EXPEDIENTE", DateTime.UtcNow, token);
                            if (estadoCambiado)
                            {
                                return RedirectToAction("Expediente");
                            }
                        }
                        throw new Exception();
                    }
                    catch (Exception)
                    {
                        ViewBag.Error = "No se pudo guardar el proceso de tu solicitud. Intente de nuevo más tarde";
                    }
                }
            }
            string bancosJson = TempData["Bancos"] as string;
            TempData["Bancos"] = bancosJson;
            IEnumerable<BancosViewModel> bancos = JsonConvert.DeserializeObject<IEnumerable<BancosViewModel>>(bancosJson);
            SelectList selectList = new(bancos, "IdBanco", "Banco");
            ViewBag.Bancos = selectList;

            string origenesJson = TempData["OrigenesInversion"] as string;
            TempData["OrigenesInversion"] = origenesJson;
            IEnumerable<OrigenInversionViewModel> origenes = JsonConvert.DeserializeObject<IEnumerable<OrigenInversionViewModel>>(origenesJson);
            selectList = new(origenes, "IdOrigen", "NombreOrigen");
            ViewBag.OrigenesInversion = selectList;

            string tiposInversionJson = TempData["TiposInversion"] as string;
            TempData["TiposInversion"] = tiposInversionJson;
            IEnumerable<TipoInversionViewModel> tiposInversion = JsonConvert.DeserializeObject<IEnumerable<TipoInversionViewModel>>(tiposInversionJson);
            selectList = new(tiposInversion, "IdTipo", "Nombre");
            ViewBag.TiposInversion = selectList;
            return View(modelo);
        }

        [HttpPost]
        public async Task<JsonResult> HandleCodigoPostalChange(string codigoPostal)
        {
            var direccionIp = ObtenerDireccionIp();
            try
            {
                var coloniasView = await _usuarios.GetColoniasPorCodigoPostalAsync(direccionIp, codigoPostal);
                return Json(new { colonias = coloniasView });
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> EnviarFirma(string base64url)
        {
            var token = Request.Cookies["Token"];
            var idInversionista = Int32.Parse(Request.Cookies["IdInversionista"]);
            try
            {
                var contratoActualizado = await _usuarios.AgregarContratoCompletoContratoInversionAsync(base64url, idInversionista, token);
                if (contratoActualizado?.Contrato != base64url)
                {
                    throw new Exception();
                }
                else
                {
                    return Json(new { exito = true });
                }
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public async Task<IActionResult> VerificarCorreo(int folioInversion, string hash)
        {
            try
            {
                var contrato = await _usuarios.ObtenerContratoPorFolioInversion(folioInversion);
                if (contrato != null && contrato.InformacionContrato != null)
                {
                    int idInversionista = (int)contrato.InformacionContrato.IdInversionista;
                    var hashId = GetSHA256(idInversionista.ToString());
                    if (hashId == hash)
                    {
                        var verificacionExitosa = await _usuarios.AgregarVerificacionesCorreo(idInversionista);
                    }
                    else
                    {
                        ViewBag.Error = "El enlace de verificación ingresado es incorrecto, favor de verificarlo.";
                    }
                }
                else
                {
                    ViewBag.Error = "Ocurrió un error al verificar el correo electrónico. Intente más tarde.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        public IActionResult AcuerdoOrigenDeFondos()
        {
            return View();
        }
        public IActionResult ContratoDeInversion()
        {
            return View();
        }
        public IActionResult Finalizado()
        {
            ViewBag.Folio = "70";
            ViewBag.Firma = "data:image/png;base64,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";
               return View();
        }
        private static string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public enum NivelEstudios
        {
            Primaria,
            Secundaria,
            Bachillerato,
            TSU,
            Licenciatura,
            Ingenieria,
            Maestria,
            Doctorado
        }
        private string ObtenerDireccionIp()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
            //return "147852";
        }
    }
}
