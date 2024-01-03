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
                        SameSite = SameSiteMode.Strict,
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
        [HttpPost]
        public async Task<IActionResult> AgregarBanco(string nombreBanco)
        {
            string token = Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.RegistrarBanco(nombreBanco, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AdministrarBancos));
            }
            if (codigoEstado == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine(codigoEstado);
                return Redirect("/admin");
            }
            else
            {
                return NotFound();
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
        [HttpPost]
        public async Task<IActionResult> EditarBanco(BancosViewModel bancoEditar)
        {
            string token = Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EditarBanco(bancoEditar, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AdministrarBancos));
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
        [HttpPost]
        public async Task<IActionResult> EliminarBanco(int idBanco)
        {
            string token = Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EliminarBanco(idBanco, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(AdministrarBancos));
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
    }
}
