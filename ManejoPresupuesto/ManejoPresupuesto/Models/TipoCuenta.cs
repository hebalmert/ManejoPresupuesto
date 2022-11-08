﻿using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "La Longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name = "Nombre del Tipo Cuenta")]
        //[PrimeraLetraMayuscula]
        public string? Nombre { get; set; }

        public int? UsuarioId { get; set; }

        public int Orden { get; set; }


        //Personalizacion de Validaciones por Modelo
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
        {
            if (Nombre != null && Nombre.Length > 0)
            { 
                var primeraLeta = Nombre[0].ToString();
                if (primeraLeta != primeraLeta.ToUpper())
                {
                    yield return new ValidationResult("La Primera Letra debe ser Mayuscula", 
                        new[] { nameof(Nombre) });
                }
            }
        }


        /* Pruebas de Otras Validaciones por Defecto */
        //[Required(ErrorMessage = "El Campo {0} es requerido")]
        //[EmailAddress(ErrorMessage = "El Campo debe ser un correo valido")]
        //public string? Email { get; set; }

        //[Range(minimum: 18, maximum: 130, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        //public int edad { get; set; }

        //[Url(ErrorMessage ="El campo debe ser una URL valida")]
        //public string? URl { get; set; }

        //[CreditCard(ErrorMessage ="La tarjeta de credito no es valida")]
        //[Display(Name ="Tarjeta de Credito")]
        //public string? tarjetadeCredito { get; set; }
    }
}