using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta /*: IValidatableObject*/  //se activa esto para poder implemntar una Validacion por Modelo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "La Longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name = "Nombre del Tipo Cuenta")]
        [PrimeraLetraMayuscula]  //Validacicion por Atributo
        [Remote(action: "VerificarExiste", controller:"TiposCuentas")]//creando una validacion desde el Controlador
                                                                      //llama el metodo en el controlados y devuelve un Json
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }

        public int Orden { get; set; }


        ////Personalizacion de Validaciones por Modelo
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
        //{
        //    if (Nombre != null && Nombre.Length > 0)
        //    { 
        //        var primeraLeta = Nombre[0].ToString();
        //        if (primeraLeta != primeraLeta.ToUpper())
        //        {
        //            //Si se coloca de esta manera, el error se aplica solo para un campo 
        //            //especifico.
        //            yield return new ValidationResult("La Primera Letra debe ser Mayuscula", 
        //                new[] { nameof(Nombre) });

        //            //Si se coloca de esta manera, el error se considera para todos los campos
        //            //Es decir, aplica para todo el modelo
        //            yield return new ValidationResult("La Primera Letra debe ser Mayuscula");
        //        }
        //    }
        //}


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
