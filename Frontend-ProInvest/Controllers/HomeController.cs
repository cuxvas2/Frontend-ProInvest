using Frontend_ProInvest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Frontend_ProInvest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<TipoInversionViewModel> listaViewModel = ObtenerListaViewModel();
            SelectList selectList = new SelectList(listaViewModel, "Rendimiento", "Nombre");
            ViewBag.TiposInversionList = selectList;
            return View();
        }
        public IActionResult AcuerdoOrigenDeFondos()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            List<TipoInversionViewModel> listaViewModel = ObtenerListaViewModel();
            SelectList selectList = new SelectList(listaViewModel, "Rendimiento", "Nombre");
            ViewBag.TiposInversionList = selectList;
            return View();
        }

        private List<TipoInversionViewModel> ObtenerListaViewModel()
        {
            // Lógica para obtener la lista de objetos ViewModel, sería Método GET con ObtenerTiposInversion
            return new List<TipoInversionViewModel>
            {
                new TipoInversionViewModel { IdTipo = 1, Nombre = "CETES", Descripcion = "", Rendimiento =  1.2},
                new TipoInversionViewModel { IdTipo = 2, Nombre = "Bonos", Descripcion = "", Rendimiento =  2.0 },
            };
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}