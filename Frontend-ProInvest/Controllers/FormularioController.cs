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
                else
                {
                    return RedirectToAction("DatosPersonales");
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
                    }
                }
                else
                {
                    return RedirectToAction("DatosPersonales");
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
                    }
                }
                else
                {
                    return RedirectToAction("DatosPersonales");
                }
                var token = Request.Cookies["Token"];
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
                if (ModelState.IsValid)
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
                    Response.Cookies.Append("FolioContrato", solicitudExistente.InformacionContrato.FolioInversion.ToString(), cookieOptions);
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
                else
                {
                    return RedirectToAction("DatosPersonales");
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
                    var folioInversion = Request.Cookies["FolioContrato"];
                    try
                    {
                        var contrato = await _usuarios.ObtenerContratoPorFolioInversion(Int32.Parse(folioInversion));
                        ViewBag.Folio = contrato.InformacionContrato.FolioInversion;
                        ViewBag.Firma = contrato.InformacionContrato.Contrato;
                        return View("Finalizado");
                    }
                    catch(Exception)
                    {
                    }
                    
                }
            }
            var listaDocumentos = await _administrador.ObtenerDocumentosExpediente(token);
            return View(listaDocumentos);

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
        }
    }
}
