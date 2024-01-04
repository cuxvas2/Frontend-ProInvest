using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using System.Security.Cryptography;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Frontend_ProInvest.Controllers
{
    public class AdminController : Controller
    {
        public string Mensaje { get; set; }
        private readonly IAdministrador _administrador;
        public AdminController(IAdministrador administrador)
        {
            _administrador = administrador;
        }

        public IActionResult Menu()
        {
            string token = Request.Cookies["tokenAdministrador"];
            if (token == null)
            {
                return Redirect("/admin");
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> AdministrarBancos()
        {
            string token = Request.Cookies["tokenAdministrador"];
            if (token == null)
            {
                return Redirect("/admin");
            }
            else
            {
                var listaBancos = await _administrador.ObtenerBancos(token);
                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                return View(listaBancos);
            }
        }
        public IActionResult CerrarSesion()
        {

            return Redirect("/admin");

        }
        [HttpGet]
        public IActionResult InicioSesion()
        {
            var viewModel = new CredencialesAccesoViewModel
            {
                Mensaje = " "
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> InicioSesion(CredencialesAccesoViewModel credencialesAcceso)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CredencialesAccesoViewModel
                {
                    Mensaje = " "
                };
                return View(viewModel);
            }
            else
            {
                string usuario = credencialesAcceso.Usuario;
                string contrasena = Encriptor(credencialesAcceso.Contrasena);
                credencialesAcceso.Usuario = usuario;
                credencialesAcceso.Contrasena = contrasena;
                CredencialesRespuestaJson credencialesObtenidas = await _administrador.ObtenerCredencialesAccesoAsync(credencialesAcceso);
                if (credencialesObtenidas.CodigoStatus == HttpStatusCode.OK)
                {
                    var viewModel = new CredencialesAccesoViewModel
                    {
                        Mensaje = "Bienvenido"
                    };
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Path = "/admin"
                    };

                    Response.Cookies.Append("tokenAdministrador", credencialesObtenidas.Token, cookieOptions);
                    return Redirect("/admin/bancos");
                }
                else
                {
                    var viewModel = new CredencialesAccesoViewModel
                    {
                        Mensaje = "Verifica tus credenciales de acceso de adminisrador"
                    };
                    return View(viewModel);
                }
            }

        }
        [HttpGet]
        public IActionResult AgregarBanco()
        {
            return View();
        }
        [HttpPost, ActionName("AgregarBancoPost")]
        public async Task<IActionResult> AgregarBanco(BancosViewModel bancoAgregar)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.RegistrarBanco(bancoAgregar.Banco, token);
            if (codigoEstado == HttpStatusCode.Created)
            {
                return Redirect("/admin/bancos");
            }
            if (codigoEstado == HttpStatusCode.BadRequest)
            {
                TempData["Error"] = "El banco ya existe.";
                return RedirectToAction("AdministrarBancos");
            }
            if (codigoEstado == HttpStatusCode.Unauthorized)
            {
                return Redirect("/admin");
            }
            else
            {
                return Redirect("/admin");
            }
        }
        [HttpGet]
        public IActionResult EditarBanco(int idBanco, string nombreBanco)
        {
            BancosViewModel bancoEditar = new BancosViewModel
            {
                IdBanco = idBanco,
                Banco = nombreBanco
            };
            return View(bancoEditar);
        }
        [HttpPost, ActionName("EditarBancoPost")]
        public async Task<IActionResult> EditarBanco(int id, BancosViewModel bancoEditar)
        {
            bancoEditar.IdBanco = id;
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EditarBanco(bancoEditar, token);
            if (codigoEstado == HttpStatusCode.OK)
            {   
                return Redirect("/admin/bancos");
            }
            if (codigoEstado == HttpStatusCode.Unauthorized)
            {
                return Redirect("/admin");
            }
            else
            {
                return Redirect("/admin");
            }
        }
        [HttpGet]
        public IActionResult EliminarBanco(int idBanco, string nombreBanco)
        {
            BancosViewModel bancoEliminar = new BancosViewModel
            {
                IdBanco = idBanco,
                Banco = nombreBanco
            };
            return View(bancoEliminar);
        }
        [HttpPost, ActionName("EliminarBancoPost")]
        public async Task<IActionResult> EliminarBanco(int id)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EliminarBanco(id, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return Redirect("/admin/bancos");
            }
            if (codigoEstado == HttpStatusCode.Unauthorized)
            {
                return Redirect("/admin");
            }
            else
            {
                return NotFound();
            }
        }
        public static string Encriptor(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder encriptedPassword = new StringBuilder();
                for (int i = 0; i < (bytes.Length); i++)
                {
                    encriptedPassword.Append(bytes[i].ToString("x2"));
                }
                return encriptedPassword.ToString().ToUpper();
            }
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

            string token = HttpContext.Request.Cookies["tokenAdministrador"];
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
                string token = HttpContext.Request.Cookies["tokenAdministrador"];
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

            string token = HttpContext.Request.Cookies["tokenAdministrador"];
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
                string token = HttpContext.Request.Cookies["tokenAdministrador"];
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

            string token = HttpContext.Request.Cookies["tokenAdministrador"];
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

            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var eliminadoExitoso = await _administrador.EliminarTipoInversionAsync(token, id);
            if (!eliminadoExitoso)
            {
                //MOstrar aviso de no exitoso
            }
            return RedirectToAction(nameof(AdministrarTiposDeInversion));
        }

    }
}
