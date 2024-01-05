using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Frontend_ProInvest.Services.Backend
{
    public class AmazonS3 : IAmazonS3
    {
        private readonly IConfiguration _configuration;
        private const string NombreBucket = "documentsproinvestlatam";

        private readonly AmazonS3Client amazonS3Cliente;

        public AmazonS3(IConfiguration configuration)
        {
            _configuration = configuration;
            var opcionesAws = _configuration.GetSection("AWS");
            var llaveAcceso = opcionesAws["AccessKey"];
            var llaveSecreta = opcionesAws["SecretKey"];

            var credenciales = new Amazon.Runtime.BasicAWSCredentials(llaveAcceso, llaveSecreta);
            var configuracion = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast2
            };

            amazonS3Cliente = new AmazonS3Client(credenciales, configuracion);
        }

        public async Task<bool> SubirArchivo(string nombreArchivo, IFormFile archivo)
        {
            try
            {
                using (var fileStream = archivo.OpenReadStream())
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = NombreBucket,
                        Key = nombreArchivo,
                        InputStream = fileStream
                    };
                    await amazonS3Cliente.PutObjectAsync(putRequest);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ObtenerUrlArchivo(string nombreArchivo)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = NombreBucket,
                    Key = nombreArchivo,
                    Expires = DateTime.Now.AddSeconds(604800)
                };

                var url = amazonS3Cliente.GetPreSignedURL(request);
                return url;
            }
            catch
            {
                return null;
            }
        }



    }
}
