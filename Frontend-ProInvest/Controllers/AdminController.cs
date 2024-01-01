using Microsoft.AspNetCore.Mvc;
using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using System.Security.Claims;

namespace Frontend_ProInvest.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdministrador _administrador;

        public AdminController(IAdministrador administrador)
        {
            _administrador = administrador;
        }
        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult SolicitudesInversiones()
        {
            SolicitudInversionViewModel solicitud = new SolicitudInversionViewModel();
            solicitud.FolioInversion = 3354;
            solicitud.NombreCompleto = "Victor Augusto Cuevas Barradas";
            solicitud.Estado = "En espera";


            SolicitudInversionViewModel solicitud2 = new SolicitudInversionViewModel();
            solicitud2.FolioInversion = 2144;
            solicitud2.NombreCompleto = "Alondra Cuevas Barradas";
            solicitud2.Estado = "En espera";


            List<SolicitudInversionViewModel> lista = new List<SolicitudInversionViewModel> { solicitud, solicitud2 };
            IEnumerable<SolicitudInversionViewModel> listaSolicitudes = lista;

            return View(listaSolicitudes);
        }

        public async Task<IActionResult> AdministrarTiposDeInversion()
        {

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c3VhcmlvIjp7InVzdWFyaW8iOiJQcm9JbnZlc3RMYXRhbSIsImNvbnRyYXNlbmEiOiJCQjlBREI2RThGQkI4NDBBOEQ2OEY3NEJFRjhEQkNCNzYzQTJFRUEzOEZEMjhDRDZCRDU3QzkzRjM5RkQ4REY1In0sImlhdCI6MTcwNDE0NjcyOSwiZXhwIjoxNzA0MTUzOTI5fQ.4dMQoL12K790La2Cmx4NQZGyOwC_P1D5XkADLi9ZEV8";
            var listaSolicitudes = await _administrador.GetTiposInversionAsync(token);
            return View(listaSolicitudes);
        }

        public IActionResult CrearTipoInversion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearTipoInversionAsync([Bind("Nombre,Rendimiento")] TipoInversionViewModel tipoInversion)
        {
            if (ModelState.IsValid)
            {
                //Se guarda en la bd (se envia a la api para que lo guarde)
                var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c3VhcmlvIjp7InVzdWFyaW8iOiJQcm9JbnZlc3RMYXRhbSIsImNvbnRyYXNlbmEiOiJCQjlBREI2RThGQkI4NDBBOEQ2OEY3NEJFRjhEQkNCNzYzQTJFRUEzOEZEMjhDRDZCRDU3QzkzRjM5RkQ4REY1In0sImlhdCI6MTcwNDE0NjcyOSwiZXhwIjoxNzA0MTUzOTI5fQ.4dMQoL12K790La2Cmx4NQZGyOwC_P1D5XkADLi9ZEV8";
                var guardadaCorrectamente = await _administrador.PostTiposInversionAsync(token, tipoInversion);
                if (guardadaCorrectamente)
                {
                    return RedirectToAction(nameof(AdministrarTiposDeInversion));  
                }
            }
            return View(tipoInversion);
        }
    }
}
