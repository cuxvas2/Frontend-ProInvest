using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Frontend_ProInvest.Controllers
{
    public class FormularioController : Controller
    {
        private readonly IUsuarios _usuarios;
        public FormularioController(IUsuarios usuarios)
        {
            _usuarios = usuarios;
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
                if(solicitudExistente?.InformacionContrato != null)
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
                            break;
                        case "FINALIZADO":
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
                        //enviar sms
                        var correoEnviado = await _usuarios.EnviarCorreoVerificacion(resultado.IdInversionista, contrato.InformacionContrato.FolioInversion, resultado.Token);
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
                var direccionIp = ObtenerDireccionIp()  ;
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
                            break;
                        case "FINALIZADO":
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
                        var estadoActualizado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(solicitudExistente.InformacionContrato.IdInversionista, "DOMICILIO", DateTime.UtcNow, solicitudExistente.Token);
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
       /* [HttpPost]
        public async Task<IActionResult> VerificacionDatosContacto(string codigo, string BtnPrevious, string BtnNext)
        {
            if (BtnNext != null)
            {
                try
                {
                    //Comparar con el código que se envió
                    //verificar SMS añadirVerificacionSMS
                    //if (verificacionCorrecta)
                    //{
                    //ViewBag.ExitoVerificacionSms = "Se ha verificado correctamente el número de celular."
                    var direccionIp = ObtenerDireccionIp();
                    var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                        Token = solicitudExistente.Token;
                        if (solicitudExistente?.InformacionContrato != null)
                        {
                            if (solicitudExistente.InformacionContrato.CorreoVerificacion == true)
                            {
                                ViewBag.CorreoVerificacion = true;
                                var estadoCambiado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(IdInversionista, "FINANCIERO", DateTime.UtcNow, Token);
                                if (estadoCambiado)
                                {
                                    return RedirectToAction("InformacionBancaria");
                                }
                                else
                                {
                                    ViewBag.Error = "No se pudo guardar el proceso de tu solicitud. Intente de nuevo más tarde";
                                }
                            }
                            else
                            {
                                ViewBag.Error = "Debes verificar tu correo electrónico para continuar";
                            }
                        }
                    //}
                    //else
                    //{
                    //  ViewBag.Error = "Ocurrió un error al verificar tu cuenta, por favor intenta de nuevo.";
                    //}
                }
                catch (Exception)
                {
                    ViewBag.Error = "Ocurrió un error al verificar tu cuenta, por favor intenta de nuevo.";
                }
            }
            return View();
        }
       */
        [HttpPost]
        public async Task<IActionResult> CorreoVerificado(string BtnContinuar)
        {
            if(BtnContinuar != null)
            {
                try
                {
                    var direccionIp = ObtenerDireccionIp();
                    var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                    if (solicitudExistente?.InformacionContrato != null)
                    {
                        if(solicitudExistente.InformacionContrato.CorreoVerificacion == true)
                        {
                            var estadoActualizado = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(solicitudExistente.InformacionContrato.IdInversionista, "DOMICILIO", DateTime.UtcNow, solicitudExistente.Token);
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
                catch(Exception ex)
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
                            break;
                        case "FINALIZADO":
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
            if(BtnNext!= null)
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
                catch(Exception ex) 
                {
                    ViewBag.Error = ex.Message;
                }
            }
            return View();
        }
        public async Task<ActionResult> InformacionBancaria()
        {
            return View("InformacionBancaria");  
        }

        [HttpPost]
        public ActionResult InformacionBancaria(InformacionBancariaViewModel modelo, string BtnPrevious, string BtnNext)
        {
            if(BtnNext!= null)
            {
                if(ModelState.IsValid)
                {
                    if(modelo.OrigenLicito == false)
                    {
                        ModelState.AddModelError("OrigenLicito", "Debe aceptar el Acuerdo de Origen de Fondos para continuar.");
                    }
                    if(modelo.AceptaContrato == false)
                    {
                        ModelState.AddModelError("AceptaContrato", "Debe aceptar el Contrato de inversión para continuar.");
                    }
                }
                if(!ModelState.IsValid)
                {
                    return View(modelo);
                }
                else
                {
                    return RedirectToAction("Direccion");
                }
            }
            return View();
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
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public async Task<IActionResult> VerificarCorreo(int folioInversion, string hash)
        {
            try
            {
                var contrato = await _usuarios.ObtenerContratoPorFolioInversion(folioInversion);
                if(contrato != null && contrato.InformacionContrato != null)
                {
                    int idInversionista = contrato.InformacionContrato.IdInversionista;
                    var hashId = GetSHA256(idInversionista.ToString());
                    if(hashId == hash)
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
            catch(Exception ex) 
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        public IActionResult AcuerdoOrigenDeFondos()
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
            //return HttpContext.Connection.RemoteIpAddress.ToString();
            return "12";
        }
    }
}
