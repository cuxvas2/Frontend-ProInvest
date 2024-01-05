﻿using Frontend_ProInvest.Models;
using Frontend_ProInvest.Services.Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Common;
using System.Diagnostics;

namespace Frontend_ProInvest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarios _usuarios;

        public HomeController(ILogger<HomeController> logger, IUsuarios usuarios)
        {
            _logger = logger;
            _usuarios = usuarios;
        }

        public async Task<IActionResult> Index()
        {
            var tiposInversion = await _usuarios.ObtenerTiposInversionAsync();
            if (tiposInversion?.Count() > 0)
            {
                SelectList selectList = new(tiposInversion, "Rendimiento", "Nombre");
                ViewBag.TiposInversionList = selectList;
            }
            else
            {
                ViewBag.Error = "No se pudieron recuperar los tipos de inversión";
            }
            return View();
        }
        public IActionResult AcuerdoOrigenDeFondos()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            List<TipoInversionViewModel> listaViewModel = ObtenerListaViewModel();
            SelectList selectList = new SelectList(listaViewModel, "Rendimiento", "Nombre");
            ViewBag.TiposInversionList = selectList;
            return View();
        }

        private List<TipoInversionViewModel> ObtenerListaViewModel()
        {
            // Lógica para obtener la lista de objetos ViewModel, sería Método GET con ObtenerTiposInversion
            return new List<TipoInversionViewModel>
            {
                new TipoInversionViewModel { IdTipo = 1, Nombre = "CETES", Descripcion = "", Rendimiento =  1.2},
                new TipoInversionViewModel { IdTipo = 2, Nombre = "Bonos", Descripcion = "", Rendimiento =  2.0 },
            };
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}