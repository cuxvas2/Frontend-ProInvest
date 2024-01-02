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
                DireccionIp = "127.0.0.1"
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
                DireccionIp = "127.0.0.1"
            };
            var inversionistaObtenido = await Usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var obtenido = await Usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            Assert.IsTrue(obtenido);
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
                DireccionIp = "127.0.0.4"
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
    }
}