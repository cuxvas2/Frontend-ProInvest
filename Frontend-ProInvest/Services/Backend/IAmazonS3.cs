using System;
using Microsoft.AspNetCore.Http;
namespace Frontend_ProInvest.Services.Backend
{
    public interface IAmazonS3
    {
        public Task<bool> SubirArchivo(string nombreArchivo, IFormFile archivo);
        public string ObtenerUrlArchivo(string nombreArchivo);
    }
}
