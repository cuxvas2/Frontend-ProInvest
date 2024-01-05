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
        private Usuarios _usuarios { get; set; }
        private readonly string DireccionIP = "25";
        public UsuariosTests()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["UrlWebAPIInversionista"]).Returns("http://localhost:8081/apiproinvest/inversionista");
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _usuarios = new Usuarios(configuration.Object, httpClientFactory.Object);
        }
        [TestMethod()]
        public async Task GetEstadosAsyncTest()
        {
            var estados = await _usuarios.GetEstadosAsync();
            Assert.AreEqual(estados.Count, 32);
        }

        [TestMethod()]
        public async Task GetColoniasPorCodigoPostalAsyncTest()
        {
            var colonias = await _usuarios.GetColoniasPorCodigoPostalAsync("127.0.0.1", "10010");
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
                DireccionIp = "1.0.0." + DireccionIP
            };
            var obtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(esperado);
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
                DireccionIp = "2.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var obtenido = await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            Assert.AreEqual(inversionistaObtenido.IdInversionista, obtenido.InformacionContrato.IdInversionista);
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
                DireccionIp = "3.0.1." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await _usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
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
                DireccionIp = "4.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await _usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var estadoObtenido = await _usuarios.EditarEstadoUltimaActualizacionContratoInversionAsync(inversionistaObtenido.IdInversionista, "DOMICILIO", DateTime.UtcNow, contratoObtenido.Token);
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
                DireccionIp = "5.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await _usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var verificacionObtenida = await _usuarios.AgregarVerificacionesCorreo(inversionistaObtenido.IdInversionista);
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
                DireccionIp = "6.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoObtenido = await _usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var envioExitoso = await _usuarios.EnviarCorreoVerificacion(inversionistaObtenido.IdInversionista, (int)contratoObtenido.InformacionContrato.FolioInversion, contratoObtenido.Token);
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
                DireccionIp = "7.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var contratoEsperado = await _usuarios.ObtenerContratoInversionPorIpAsync(inversionistaPrueba.DireccionIp);
            var contratoObtenido = await _usuarios.ObtenerContratoPorFolioInversion((int)contratoEsperado.InformacionContrato.FolioInversion);
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
                DireccionIp = "8.0.0." + DireccionIP
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            inversionistaObtenido.Calle = "Calle 3";
            inversionistaObtenido.Colonia = "Juarez";
            inversionistaObtenido.CodigoPostal = "12345";
            inversionistaObtenido.Estado = "Veracruz";
            inversionistaObtenido.Municipio = "Coatazacoalcos";
            inversionistaObtenido.NumeroExterior = "3";
            inversionistaObtenido.DireccionIp = "127.0.0.24";
            var respuesta = await _usuarios.AnadirInformacionDomicilioInversionistaAsync(inversionistaObtenido, inversionistaObtenido.Token);
            Assert.IsTrue(respuesta.Token != null);
            Assert.AreEqual(inversionistaObtenido.Nombre, respuesta.Nombre);
        }

        [TestMethod()]
        public async Task AgregarContratoCompletoContratoInversionAsyncTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Francisco",
                ApellidoPaterno = "Fernandez",
                ApellidoMaterno = "Nuñez",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "Oracle",
                GradoAcademico = "Licenciatura",
                Profesion = "Ingeniero",
                DireccionIp = "0.0.07",
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var contratoCreado = await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            string base64Url = "data:image/png;base64,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";
            var contratoActualizado = await _usuarios.AgregarContratoCompletoContratoInversionAsync(base64Url, inversionistaObtenido.IdInversionista, inversionistaObtenido.Token);
            Assert.AreEqual(base64Url, contratoActualizado.Contrato);
        }

        [TestMethod()]
        public async Task ObtenerTiposInversionAsyncTest()
        {
            var obtenido = await _usuarios.ObtenerTiposInversionAsync();
            Assert.IsTrue(obtenido.Count() > 0);
        }
    }
}