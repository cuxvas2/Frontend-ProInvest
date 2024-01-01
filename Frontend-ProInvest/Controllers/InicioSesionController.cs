using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using System.Security.Cryptography;
using Frontend_ProInvest.Services.Backend.ModelsHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Frontend_ProInvest.Controllers
{
    public class InicioSesionController : Controller
    {
        public string Mensaje { get; set; }
        private readonly IAdministrador _administrador;
        public InicioSesionController(IAdministrador administrador)
        {
            _administrador = administrador;
        }
        private String _vista = "~/Views/Admin/InicioSesion.cshtml";
        public IActionResult Index()
        {
            var viewModel = new CredencialesAccesoViewModel
            {
                Mensaje = " "
            };
            return View(_vista, viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(CredencialesAccesoViewModel credencialesAcceso)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CredencialesAccesoViewModel
                {
                    Mensaje = " "
                };
                return View(_vista, viewModel);
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
                    return View(_vista, viewModel);
                }
                else
                {
                    var viewModel = new CredencialesAccesoViewModel
                    {
                        Mensaje = "Verifica tus credenciales de acceso de adminisrador"
                    };
                    return View(_vista, viewModel);
                }
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
