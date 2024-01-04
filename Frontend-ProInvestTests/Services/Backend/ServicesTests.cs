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
    public class ServicesTests
    {
        private Usuarios _usuarios { get; set; }
        private Administrador _administrador { get; set; }
        public ServicesTests()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["UrlWebAPIInversionista"]).Returns("http://localhost:8081/apiproinvest/inversionista");
            configuration.Setup(c => c["UrlWebAPIAdministrador"]).Returns("http://localhost:8081/apiproinvest/admin");
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _usuarios = new Usuarios(configuration.Object, httpClientFactory.Object);
            _administrador = new Administrador(configuration.Object, httpClientFactory.Object);
        }
        [TestMethod()]
        public async Task CrearInformacionBancariaTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Silvia",
                ApellidoPaterno = "Ignacio",
                ApellidoMaterno = "Ruiz",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "UV",
                GradoAcademico = "Primaria",
                Profesion = "Chef",
                DireccionIp = "9.0.0.03",
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var contratoCreado = await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var bancos = await _administrador.ObtenerBancos(inversionistaObtenido.Token);
            var idBanco = bancos?.FirstOrDefault()?.IdBanco ?? 2;
            var origenes = await _administrador.ObtenerOrigenesInversion(inversionistaObtenido.Token);
            var idOrigen = origenes?.OrigenesInversion?.FirstOrDefault()?.IdOrigen ?? 1;
            var idTipo = 5;
            InformacionBancariaViewModel informacionBancaria = new InformacionBancariaViewModel()
            {
                OrigenDeFondos = "Ahorros",
                Cuenta = "1234567890",
                ClabeInterbancaria = "123456789012345678",
                Banco = "Banamex",
                CantidadAInvertir = 100000,
                Anios = 2,
                TipoDeInversion = "Cetes",
                AceptaContrato = true,
                OrigenLicito = true,
                IdBanco = idBanco,
                IdOrigen = idOrigen,
                IdTipo = idTipo
            };
            var informacionBancariaCreada = await _usuarios.CrearInformacionBancaria(informacionBancaria, (int)contratoCreado.InformacionContrato.FolioInversion, inversionistaObtenido.Token);
            Assert.IsTrue(informacionBancariaCreada);
        }
        [TestMethod()]
        public async Task EditarInversionContratoInversionTest()
        {
            InversionistaViewModel inversionistaPrueba = new InversionistaViewModel()
            {
                Nombre = "Rodrigo",
                ApellidoPaterno = "Fernandez",
                ApellidoMaterno = "Nuñez",
                CorreoElectronico = "cstllsarai@gmail.com",
                TelefonoCelular = "2281234567",
                FechaNacimiento = DateTime.Now,
                Rfc = "123456789123",
                EmpresaTrabajo = "Oracle",
                GradoAcademico = "Licenciatura",
                Profesion = "Ingeniero",
                DireccionIp = "10.0.0.06",
            };
            var inversionistaObtenido = await _usuarios.AnadirInformacionPersonalInversionistaAsync(inversionistaPrueba);
            var contratoCreado = await _usuarios.CrearContratoInversionAsync(inversionistaPrueba.DireccionIp, inversionistaObtenido.IdInversionista, DateTime.UtcNow);
            var bancos = await _administrador.ObtenerBancos(inversionistaObtenido.Token);
            var idBanco = bancos?.FirstOrDefault()?.IdBanco ?? 2;
            var origenes = await _administrador.ObtenerOrigenesInversion(inversionistaObtenido.Token);
            var idOrigen = origenes?.OrigenesInversion?.FirstOrDefault()?.IdOrigen ?? 1;
            var idTipo = 5;
            InformacionBancariaViewModel informacionBancaria = new InformacionBancariaViewModel()
            {
                OrigenDeFondos = "Ahorros",
                Cuenta = "1234567890",
                ClabeInterbancaria = "123456789012345678",
                Banco = "Banamex",
                CantidadAInvertir = 100000,
                Anios = 2,
                TipoDeInversion = "Cetes",
                AceptaContrato = true,
                OrigenLicito = true,
                IdBanco = idBanco,
                IdOrigen = idOrigen,
                IdTipo = idTipo
            };
            var contratoActualizado = await _usuarios.EditarInversionContratoInversion(informacionBancaria, inversionistaObtenido.IdInversionista, inversionistaObtenido.Token);
            Assert.AreEqual(inversionistaObtenido.IdInversionista, contratoActualizado.IdInversionista);
            Assert.AreEqual(informacionBancaria.CantidadAInvertir, contratoActualizado.Importe);
        }
    }
}
