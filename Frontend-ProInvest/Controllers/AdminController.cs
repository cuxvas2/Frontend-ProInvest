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
                    return Redirect("/admin/menu");
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
        [HttpGet]
        public async Task<IActionResult> SolicitudesInversiones()
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var listacontratos = await _administrador.ObtenerContratos(token);
            return View(listacontratos);
        }

        [HttpGet]
        public async Task<IActionResult> AdministrarTiposDeInversion()
        {

            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var listaTipoInversiones = await _administrador.GetTiposInversionAsync(token);
            return View(listaTipoInversiones);
        }

        public async Task<IActionResult> DetallesDeSolicitudDeInversiones(int folio)
        {
            if (folio == 0)
            {
                return NotFound();
            }
            Console.WriteLine(folio);
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var contrato = await _administrador.ObtenerInformacionContratoPorFolio(token, folio);
            var solicitudInversion = await _administrador.ObtenerSolicitudInversion(token, contrato);
            return View(solicitudInversion);
        }

        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
            return RedirectToAction(nameof(AdministrarTiposDeInversion), id);
        }
        public async Task<IActionResult> GestionarOrigenesInversion()
        {
            string token = Request.Cookies["tokenAdministrador"];
            if (token == null)
            {
                return Redirect("/admin");
            }
            else
            {
                var listaOrigenesInversionToken = await _administrador.ObtenerOrigenesInversion(token);
                var listaOrigenesInversion = listaOrigenesInversionToken.OrigenesInversion;
                IEnumerable<OrigenInversionViewModel> listaOrigenesInversionViewModel = listaOrigenesInversion;
                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                return View(listaOrigenesInversionViewModel);
            }
        }
        [HttpGet]
        public IActionResult AgregarOrigenInversion()
        {
            return View();
        }
        [HttpPost, ActionName("AgregarOrigenInversionPost")]
        public async Task<IActionResult> AgregarOrigenInversion(OrigenInversionViewModel origenAgregar)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.RegistrarOrigenInversion(origenAgregar.NombreOrigen, token);
            if (codigoEstado == HttpStatusCode.Created)
            {
                return Redirect("/admin/origenesInversion");
            }
            if (codigoEstado == HttpStatusCode.BadRequest)
            {
                TempData["Error"] = "El origen ya existe.";
                return RedirectToAction("GestionarOrigenesInversion");
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
        public IActionResult EditarOrigenInversion(int idOrigenInversion, string nombreOrigen)
        {
            OrigenInversionViewModel origenInversion = new OrigenInversionViewModel
            {
                IdOrigen = idOrigenInversion,
                NombreOrigen = nombreOrigen
            };
            return View(origenInversion);
        }
        [HttpPost, ActionName("EditarOrigenInversionPost")]
        public async Task<IActionResult> EditarOrigenInversion(int id, OrigenInversionViewModel origenEditar)
        {
            origenEditar.IdOrigen = id;
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EditarOrigenInversion(origenEditar, token);
            if (codigoEstado == HttpStatusCode.OK)
            {   
                return Redirect("/admin/origenesInversion");
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
        public IActionResult EliminarOrigenInversion(int idOrigen, string nombreOrigen)
        {
            OrigenInversionViewModel origenInversion = new OrigenInversionViewModel
            {
                IdOrigen = idOrigen,
                NombreOrigen = nombreOrigen
            };
            return View(origenInversion);
        }
        [HttpPost, ActionName("EliminarOrigenInversionPost")]
        public async Task<IActionResult> EliminEliminarOrigenInversionarBanco(int id)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EliminarOrigenInversion(id, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return Redirect("/admin/origenesInversion");
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
        public async Task<IActionResult> GestionarListaDocumentos()
        {
            string token = Request.Cookies["tokenAdministrador"];
            if (token == null)
            {
                return Redirect("/admin");
            }
            else
            {
                IEnumerable<DocumentosExpedienteViewModel> listaDocumentos = await _administrador.ObtenerDocumentosExpediente(token);
                if (TempData["Error"] != null)
                {
                    ViewBag.Error = TempData["Error"].ToString();
                }
                return View(listaDocumentos);
            }
        }
        [HttpGet]
        public IActionResult AgregarDocumentosExpediente()
        {
            return View();
        }
        [HttpPost, ActionName("AgregarDocumentosExpedientePost")]
        public async Task<IActionResult> AgregarDocumentosExpediente(DocumentosExpedienteViewModel documentoAgregar)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.RegistrarDocumento(documentoAgregar.NombreDocumento, token);
            if (codigoEstado == HttpStatusCode.Created)
            {
                return Redirect("/admin/listaDocumentos");
            }
            if (codigoEstado == HttpStatusCode.BadRequest)
            {
                TempData["Error"] = "El documento ya existe.";
                return RedirectToAction("GestionarListaDocumentos");
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
        public IActionResult EditarDocumentosExpediente(int idDocumento, string nombreDocumento)
        {
            DocumentosExpedienteViewModel documento = new DocumentosExpedienteViewModel
            {
                IdDocumento = idDocumento,
                NombreDocumento = nombreDocumento
            };
            return View(documento);
        }
        [HttpPost, ActionName("EditarDocumentosExpedientePost")]
        public async Task<IActionResult> EditarDocumentosExpediente(int id, DocumentosExpedienteViewModel documentoEditar)
        {
            documentoEditar.IdDocumento = id;
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EditarDocumento(documentoEditar, token);
            if (codigoEstado == HttpStatusCode.OK)
            {   
                return Redirect("/admin/listaDocumentos");
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
        public IActionResult EliminarDocumentoExpediente(int idDocumento, string nombreDocumento)
        {
            DocumentosExpedienteViewModel documento = new DocumentosExpedienteViewModel
            {
                IdDocumento = idDocumento,
                NombreDocumento = nombreDocumento
            };
            return View(documento);
        }
       
        [HttpPost, ActionName("EliminarDocumentoExpedientePost")]
        public async Task<IActionResult> EliminarDocumentoExpediente(int id)
        {
            string token = HttpContext.Request.Cookies["tokenAdministrador"];
            var codigoEstado = await _administrador.EliminarDocumento(id, token);
            if (codigoEstado == HttpStatusCode.OK)
            {
                return Redirect("/admin/listaDocumentos");
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
    }
}
