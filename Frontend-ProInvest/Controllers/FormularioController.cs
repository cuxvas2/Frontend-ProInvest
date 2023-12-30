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
            var viewModel = new DatosPersonalesViewModel();
            ViewBag.NivelesEstudio = Enum.GetValues(typeof(DatosPersonalesViewModel.NivelEstudios));
            return View();
        }

        [HttpPost]
        public IActionResult DatosPersonales(DatosPersonalesViewModel personal, 
            string BtnPrevious, string BtnNext)
        {
            if(BtnNext != null)
            {
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
