using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class InversionistaViewModel
    {
        public int NumeroExterior {  get; set; }
        public string EmpresaTrabajo { get; set; }
        public string Profesion {  get; set; }
        public string CorreoElectronico {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string FechaNacimiento { get; set; }
        public string Rfc {  get; set; }
        public int IdInversionista { get; set; }
        public string GradoAcademico {  get; set; }
        public string TelefonoCelular { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre {  get; set; }
        public string CodigoPostal { get; set; }
        public string Calle {  get; set; }
        public string Colonia { get; set; }
        public string Estado {  get; set; }
        public int NumeroInterior {  get; set; }
        public string Municipio { get; set; }

    }
}
