using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using Microsoft.AspNetCore.Mvc;

namespace Frontend_ProInvest.Controllers
{
    public class FormularioController : Controller
    {
        private readonly IUsuarios _usuarios;
        private string Token {  get; set; }
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
                var direccionIp = HttpContext.Connection.RemoteIpAddress.ToString();
                var solicitudExistente = await _usuarios.ObtenerContratoInversionPorIpAsync(direccionIp);
                Token = solicitudExistente.Token;
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
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(InversionistaViewModel.NivelEstudios));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatosPersonalesAsync(InversionistaViewModel datosPersonales, 
            string BtnPrevious, string BtnNext)
        {
            var direccionIp = HttpContext.Connection.RemoteIpAddress.ToString();
            datosPersonales.DireccionIp = direccionIp;
            if (BtnNext != null)
            {
                try
                {
                    var resultado = await _usuarios.AnadirInformacionPersonalInversionistaAsync(datosPersonales);
                    var contrato = await _usuarios.CrearContratoInversionAsync(direccionIp, resultado.IdInversionista, DateTime.UtcNow);
                    if (resultado?.Token != null && contrato)
                    {
                        Token = resultado.Token;
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
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(InversionistaViewModel.NivelEstudios));
            return View();
        }

        public IActionResult VerificacionDatosContacto(string BtnPrevious, string BtnNext)
        {
            if(BtnNext != null)
            {
                return RedirectToAction("Direccion");
            }
            return View();
        }
        public async Task<IActionResult> Direccion()
        {
            try
            {
                var estados = await _usuarios.GetEstadosAsync();
                ViewBag.Estados = estados;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            
            return View("Direccion");
        }

        [HttpPost]
        public ActionResult Direccion(InversionistaViewModel direccion, string BtnPrevious, string BtnNext)
        {
            if(BtnNext!= null)
            {
                if(ModelState.IsValid)
                {
                    return RedirectToAction("InformacionBancaria");
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
            try
            {
                var coloniasView = await _usuarios.GetColoniasPorCodigoPostalAsync("", codigoPostal);
                return Json(new { colonias = coloniasView });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
