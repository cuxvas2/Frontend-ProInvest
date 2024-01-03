using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frontend_ProInvest.Services.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend.Tests
{
    [TestClass()]
    public class UsuariosTests
    {
        private Usuarios Usuarios { get; set; }
        public UsuariosTests()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["UrlWebAPIInversionista"]).Returns("http://localhost:8081/apiproinvest/inversionista");
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);
            Usuarios = new Usuarios(configuration.Object, httpClientFactory.Object);
        }
        [TestMethod()]
        public async Task GetEstadosAsyncTest()
        {
            var estados = await Usuarios.GetEstadosAsync();
            Assert.AreEqual(estados.Count, 32);
        }

        [TestMethod()]
        public async Task GetColoniasPorCodigoPostalAsyncTest()
        {
            var colonias = await Usuarios.GetColoniasPorCodigoPostalAsync("127.0.0.1", "10010");
            foreach (var item in colonias)
            {
                Console.WriteLine(item.ToString());
            }
            Assert.AreEqual(2, colonias.Count);
        }

        [TestMethod()]
        public async Task AnadirInformacionPersonalInversionistaAsyncTest()
        {
            InversionistaViewModel esperado = new InversionistaViewModel()
            {
                Nombre = "Sara",
                ApellidoPaterno = "Castillo",
                CorreoElectronico = "uno@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.22"
            };
            var obtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(esperado);
            Console.WriteLine("Token: " + obtenido.Token);
            Assert.IsTrue(esperado.Nombre == obtenido.Nombre && obtenido.Token != null);
        }

        [TestMethod()]
        public async Task CrearContratoInversionAsyncTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Sara",
                ApellidoPaterno = "Castillo",
                CorreoElectronico = "uno@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.21"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var obtenido = await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            Assert.AreEqual(inversionistaObtenido.IdInversionista, inversionistaObtenido.IdInversionista);
        }

        [TestMethod()]
        public async Task ObtenerContratoInversionPorIpTestAsyncExiste()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Sara",
                ApellidoPaterno = "Castillo",
                CorreoElectronico = "uno@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.1.19"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await Usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            Assert.IsTrue(
                    contratoObtenido.Token != null &&
                    contratoObtenido.InformacionContrato != null &&
                    contratoObtenido.InformacionContrato.IdInversionista == inversionistaObtenido.IdInversionista &&
                    contratoObtenido.InformacionContrato.Estado == "VERIFICACION" &&
                    contratoObtenido.InformacionContrato.DireccionIp == inversionistaPrueba.DireccionIp
                );
        }

        [TestMethod()]
        public async Task EditarEstadoUltimaActualizacionContratoInversionTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Sara",
                ApellidoPaterno = "Castillo",
                CorreoElectronico = "uno@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.20"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await Usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var estadoObtenido = await Usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(inversionistaObtenido.IdInversionista, "DOMICILIO", DateTime.UtcNow, contratoObtenido.Token);
            Assert.IsTrue(estadoObtenido);
        }

        [TestMethod()]
        public async Task AgregarVerificacionesCorreoTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Juan Luis",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Sanchez",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.18"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await Usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var verificacionObtenida = await Usuarios.AgregarVerificacionesCorreo(inversionistaObtenido.IdInversionista);
            Assert.IsTrue(verificacionObtenida);
        }

        [TestMethod()]
        public async Task EnviarCorreoVerificacionTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Fernanda",
                ApellidoPaterno = "Hernández",
                ApellidoMaterno = "Álvarez",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.17"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await Usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var envioExitoso = await Usuarios.EnviarCorreoVerificacion(inversionistaObtenido.IdInversionista, contratoObtenido.InformacionContrato.FolioInversion, contratoObtenido.Token);
            Assert.IsTrue(envioExitoso);
        }

        [TestMethod()]
        public async Task ObtenerContratoPorFolioInversionTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Angeles",
                ApellidoPaterno = "Zarate",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.16"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoEsperado = await Usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var contratoObtenido = await Usuarios.ObtenerContratoPorFolioInversion(contratoEsperado.InformacionContrato.FolioInversion);
            Assert.IsTrue(contratoObtenido.InformacionContrato != null);
            Assert.IsTrue(contratoObtenido.InformacionContrato.IdInversionista == inversionistaObtenido.IdInversionista);
            Assert.IsTrue(contratoObtenido.InformacionContrato.FolioInversion == contratoEsperado.InformacionContrato.FolioInversion);
        }

        [TestMethod()]
        public async Task AnadirInformacionDomicilioInversionistaAsyncTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Karla",
                ApellidoPaterno = "Torres",
                ApellidoMaterno = "Juarez",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Licenciatura",
                Profesion = "Profesor",
                DireccionIp = "127.0.0.25",
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            inversionistaObtenido.Calle = "Calle 3";
            inversionistaObtenido.Colonia = "Juarez";
            inversionistaObtenido.CodigoPostal = "12345";
            inversionistaObtenido.Estado = "Veracruz";
            inversionistaObtenido.Municipio = "Coatazacoalcos";
            inversionistaObtenido.NumeroExterior = "3";
            inversionistaObtenido.DireccionIp = "127.0.0.24";
            var respuesta = await Usuarios.AnadirInformacionDomicilioInversionistaAsync(inversionistaObtenido, inversionistaObtenido.Token);
            Assert.IsTrue(respuesta.Token != null);
            Assert.AreEqual(inversionistaObtenido.Nombre, respuesta.Nombre);
        }
    }
}