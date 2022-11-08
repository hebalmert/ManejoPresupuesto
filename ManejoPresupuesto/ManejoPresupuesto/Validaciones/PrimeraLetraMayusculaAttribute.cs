using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace ManejoPresupuesto.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {

        ///Validaciones por Atributo, para que se puedan llamar en cualquier modelo
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) 
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La Primera Letra debe ser Mayuscula");

            }

            return ValidationResult.Success;
        }


    }
}
