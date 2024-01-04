using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frontend_ProInvest.Services.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Configuration;
using Frontend_ProInvest.Models;

namespace Frontend_ProInvest.Services.Backend.Tests
{
    [TestClass()]
    public class AdministradorTests
    {
        private readonly Administrador _administrador;
        private readonly string DireccionIp = "127.0.0.1";
        public AdministradorTests()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["UrlWebAPIAdministrador"]).Returns("http://localhost:8081/apiproinvest/admin");
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _administrador = new Administrador(configuration.Object, httpClientFactory.Object);
        }
        [TestMethod()]
        public async Task ObtenerOrigenesInversionTest()
        {
            var credenciales = await _administrador.ObtenerCredencialesAccesoAsync(new CredencialesAccesoViewModel { Usuario = "ProInvestLatam", Contrasena = "3E897F9D82F46F069B2BFDCB0D1F69B07C9534A3C3A8A074E881CCF5C021E986" });
            var origenesInversion = await _administrador.ObtenerOrigenesInversion(credenciales.Token);
            Assert.IsTrue(origenesInversion.OrigenesInversion.Count > 0);
            Assert.IsTrue(origenesInversion.Token != null);
        }
    }
}