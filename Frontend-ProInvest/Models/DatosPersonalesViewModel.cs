using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class DatosPersonalesViewModel
    {
        public int IdInversionista { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9&]+$", ErrorMessage = "Ingresa una empresa válida sin caracteres especiales.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Empresa*")]
        public string EmpresaTrabajo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^(?!^\s*$)[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Ingresa una profesión válida sin caracteres especiales.")]
        [Display(Name = "Profesión*")]
        public string Profesion {  get; set; }

        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Correo Electrónico*")]
        public string CorreoElectronico {  get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^(?!^\s*$)[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Ingresa un apellido válido sin caracteres especiales.")]
        [Display(Name = "Apellido Paterno*")]
        public string ApellidoPaterno { get; set; }

        [RegularExpression(@"^(?!^\s*$)[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Ingresa un apellido válido sin caracteres especiales.")]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date, ErrorMessage = "Por favor, introduce una fecha válida.")]
        [Display(Name = "Fecha de Nacimiento*")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^(?!.*[&<>']).{12}$", ErrorMessage = "El RFC debe incluir 12 caracteres.")]
        [Display(Name = "RFC*")]
        public string Rfc {  get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Grado Académico*")]
        public string GradoAcademico {  get; set; }

        [StringLength(10)]
        [RegularExpression(@"^[0-9]{10}", ErrorMessage = "Ingresa tu numero de celular a 10 dígitos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Teléfono Celular*")]
        public string TelefonoCelular { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^(?!^\s*$)[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Ingresa un nombre válido sin caracteres especiales.")]
        [Display(Name = "Nombre*")]
        public string Nombre {  get; set; }

        public enum NivelEstudios
        {
            Primaria,
            Secundaria,
            Bachillerato,
            TSU,
            Licenciatura,
            Ingenieria,
            Maestria,
            Doctorado
        }

    }
}
