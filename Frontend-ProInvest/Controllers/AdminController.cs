using Microsoft.AspNetCore.Mvc;
using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using System.Security.Claims;

namespace Frontend_ProInvest.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdministrador _administrador;
        private readonly string tokenAdmin = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c3VhcmlvIjp7InVzdWFyaW8iOiJQcm9JbnZlc3RMYXRhbSIsImNvbnRyYXNlbmEiOiJCQjlBREI2RThGQkI4NDBBOEQ2OEY3NEJFRjhEQkNCNzYzQTJFRUEzOEZEMjhDRDZCRDU3QzkzRjM5RkQ4REY1In0sImlhdCI6MTcwNDE3NzEzMywiZXhwIjoxNzA0MTg0MzMzfQ.ESGminJyw2DwkTZzAEk98zb-3wUTjKRlaYQHY-GjU3U";

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

            var token = tokenAdmin;
            var listaTipoInversiones = await _administrador.GetTiposInversionAsync(token);
            return View(listaTipoInversiones);
        }

        public IActionResult CrearTipoInversion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearTipoInversionAsync([Bind("Nombre,Descripcion,Rendimiento")] TipoInversionViewModel tipoInversion)
        {
            if (ModelState.IsValid)
            {
                //Se guarda en la bd (se envia a la api para que lo guarde)
                var token = tokenAdmin;
                var guardadaCorrectamente = await _administrador.AnadirTiposInversionAsync(token, tipoInversion);
                if (guardadaCorrectamente)
                {
                    return RedirectToAction(nameof(AdministrarTiposDeInversion));  
                }
            }
            return View(tipoInversion);
        }

        public async Task<IActionResult> EditarTipoInversion(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = tokenAdmin;
            var tipoInversion = await _administrador.GetTipoInversionAsync(token, id);
            if (tipoInversion == null)
            {
                return NotFound();
            }
            return View(tipoInversion);
        }

        [HttpPost]
        public async Task<IActionResult> EditarTipoInversion(int id, [Bind("IdTipo,Nombre,Descripcion,Rendimiento")] TipoInversionViewModel tipoInversion)
        {
            if (id != tipoInversion.IdTipo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var token = tokenAdmin;
                var guardadoExitoso = await _administrador.EditarTipoInversionAsync(token, tipoInversion);
                if (guardadoExitoso)
                {
                    return RedirectToAction(nameof(AdministrarTiposDeInversion));
                }
                else
                {
                    return View(tipoInversion);
                }
            }
            return View(tipoInversion);
        }

        public async Task<IActionResult> EliminarTipoInversion(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = tokenAdmin;
            var tipoInversion = await _administrador.GetTipoInversionAsync(token, id);
            if (tipoInversion == null)
            {
                return NotFound();
            }

            return View(tipoInversion);
        }

        [HttpPost, ActionName("EliminarTipoInversion")]
        public async Task<IActionResult> EliminarTipoInversionDelete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = tokenAdmin;
            var eliminadoExitoso = await _administrador.EliminarTipoInversionAsync(token, id);
            if (!eliminadoExitoso)
            {
                //MOstrar aviso de no exitoso
            }
            return RedirectToAction(nameof(AdministrarTiposDeInversion));
        }

    }
}
