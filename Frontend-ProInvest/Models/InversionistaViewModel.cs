using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class InversionistaViewModel
    {
        public int IdInversionista { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9&\s]+$", ErrorMessage = "Ingresa una empresa válida sin caracteres especiales.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Empresa o Escuela (si eres estudiante)*")]
        public string EmpresaTrabajo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^(?!^\s*$)[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Ingresa una profesión válida incluyendo solo letras.")]
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
        [RegularExpression(@"^(?!.*[&<>']).{13}$", ErrorMessage = "El RFC debe incluir 13 caracteres sin incluir caracteres especiales.")]
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
        
        public string DireccionIp {  get; set; }

        //Dirección

        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa una calle válida (solo letras y números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Calle*")]
        public string Calle { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+$", ErrorMessage = "Ingresa un número exterior válido (solo letras y números, sin espacios).")]
        [Display(Name = "Número exterior")]
        public string NumeroExterior { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+$", ErrorMessage = "Ingresa un número interior válido (solo letras y números, sin espacios).")]
        [Display(Name = "Número interior")]
        public string NumeroInterior { get; set; }
        [RegularExpression(@"^[0-9]{5}", ErrorMessage = "Ingresa tu código postal a 5 dígitos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Código Postal*")]
        public string CodigoPostal { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa una colonia válida (solo letras y números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Colonia*")]
        public string Colonia { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "Ingresa un estado válido (solo letras).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Estado*")]
        public string Estado { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "Ingresa un municipio válido (solo letras).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Municipio*")]
        public string Municipio { get; set; }
        public string Token { get; set; }

        public string NombreCompleto 
        { 
            get { return Nombre + " " + ApellidoPaterno + " " + ApellidoMaterno; }
        }

        public string ToString()
        {
            return "CP: " + CodigoPostal + ", Colonia: " + Colonia + ", Municipio: " +
                Municipio + ", Estado: " + Estado;
        }
    }
}
