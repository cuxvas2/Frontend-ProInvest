using Frontend_ProInvest.Models;
using Microsoft.AspNetCore.Mvc;

namespace Frontend_ProInvest.Controllers
{
    public class FormularioController : Controller
    {
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
                return View("VerificacionDatosContacto");
            }
            return View();
        }

        [HttpPost]
        public IActionResult VerificacionDatosContacto(string BtnPrevious, string BtnNext)
        {
            if(BtnNext != null)
            {
                return View("Direccion");
            }
            return View();
        }
    }
}
