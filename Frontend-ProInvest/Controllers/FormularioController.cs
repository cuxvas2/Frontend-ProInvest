using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult DatosPersonales()
        {
            var viewModel = new InversionistaViewModel();
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(InversionistaViewModel.NivelEstudios));
            return View();
        }

        [HttpPost]
        public IActionResult DatosPersonales(InversionistaViewModel personal, 
            string BtnPrevious, string BtnNext)
        {
            var ipConnectionRemote = HttpContext.Connection.RemoteIpAddress.ToString();
            if (BtnNext != null)
            {
                //hacer código Usuarios 
                /**
                 * Checar cómo está en el api
                 * Checar que necesita
                 * Recuperar y enviar lo necesario
                 */
                return RedirectToAction("VerificacionDatosContacto");
            }
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
