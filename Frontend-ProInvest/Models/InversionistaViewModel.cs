using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class InversionistaViewModel
    {
        public int NumeroExterior {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Empresa*")]
        public string EmpresaTrabajo { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Profesión*")]
        public string Profesion {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Correo Electrónico*")]
        public string CorreoElectronico {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Apellido Paterno*")]
        public string ApellidoPaterno { get; set; }
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Fecha de Nacimiento*")]
        public string FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "RFC*")]
        public string Rfc {  get; set; }
        public int IdInversionista { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Grado Académico*")]
        public string GradoAcademico {  get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Teléfono Celular")]
        public string TelefonoCelular { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre*")]
        public string Nombre {  get; set; }
        public string CodigoPostal { get; set; }
        public string Calle {  get; set; }
        public string Colonia { get; set; }
        public string Estado {  get; set; }
        public int NumeroInterior {  get; set; }
        public string Municipio { get; set; }

    }
}
