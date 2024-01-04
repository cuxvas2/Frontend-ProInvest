using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel.DataAnnotations;

namespace Frontend_ProInvest.Models
{
    public class InformacionBancariaViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Origen de fondos*")]
        public string OrigenDeFondos { get; set; }


        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[0-9]{10,13}$", ErrorMessage = "Ingresa tu numero de cuenta con 10 a 13 dígitos")]
        [Display (Name = "Número de cuenta*")]
        public string Cuenta { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "Ingresa cuenta interbancaria con 18 dígitos")]
        [Display(Name = "Clave interbancaria*")]
        public string ClabeInterbancaria { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa un banco válido (solo letras, números y espacios).")]
        [Display(Name = "Banco*")]
        public string Banco { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[1-9]\d*0000$", ErrorMessage = "Ingrese un número entero y con aumentos de $10,000")]
        [Range(10000, 2000000, ErrorMessage = "El importe debe estar entre 10,000 y 2,000,000")]
        [Display(Name = "Cantidad a invertir*")]
        public int CantidadAInvertir { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[1-5]$", ErrorMessage = "Ingrese un número entero entre 1 y 5")]
        [Range(1, 5, ErrorMessage = "Los años a invertir deben estar entre 1 y 5")]
        [Display(Name = "Años a invertir*")]
        public int Anios {  get; set; }

        [RegularExpression(@"^[a-zA-ZÀ-ÖØ-öø-ÿ0-9\s]+$", ErrorMessage = "Ingresa un tipo de inversión válido (solo letras y números).")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Tipo de inversión*")]
        public string TipoDeInversion { get; set; }

        [Required(ErrorMessage = "Debe aceptar el Contrato de inversión para continuar.")]
        [Display(Name = "He leído y estoy de acuerdo con el contrato de inversión")]
        public bool AceptaContrato { get; set; }

        [Required(ErrorMessage = "Debe aceptar el Acuerdo de Origen de Fondos para continuar.")]
        public bool OrigenLicito { get; set; }
        public int IdBanco { get; set; }
        public int IdTipo { get; set; }
        public int IdOrigen { get; set; }
    }
}
