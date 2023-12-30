using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class DireccionViewModel
    {
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa una calle válida (solo letras y números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Calle*")]
        public string Calle {  get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+$", ErrorMessage = "Ingresa un número exterior válido (solo letras y números sin espacios).")]
        [Display(Name = "Número exterior")]
        public string NumeroExterior { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+$", ErrorMessage = "Ingresa un número interior válido (solo letras y números sin espacios).")]
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
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "Ingresa un estado válido (solo letras y números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Estado*")]
        public string Estado { get; set; }
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\s]+$", ErrorMessage = "Ingresa un municipio válido (solo letras).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Municipio*")]
        public string Municipio { get; set; }
        public string Token { get; set; }

        public string ToString()
        {
            return "CP: " + CodigoPostal + ", Colonia: " + Colonia + ", Municipio: " +
                Municipio + ", Estado: " + Estado;
        }
    }
}
