﻿using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Display(Name ="Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        //public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:MM tt"));
        //public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("g"));

        public decimal Monto { get; set; }

        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una Categoria")]
        [Display(Name ="Categoria")]
        public int CategoriaId { get; set; }

        [StringLength(maximumLength:1000, ErrorMessage = "La Nota no puede pasar de {1} Caracteres")]
        public string Nota { get; set; }

        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe Seleccionar una Cuenta")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }
    }
}
