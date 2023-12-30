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
            foreach(var item in colonias)
            {
                Console.WriteLine(item.ToString());
            }
            Assert.AreEqual(2, colonias.Count);
        }
    }
}