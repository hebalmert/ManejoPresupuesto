using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage ="No Puede ser mayor a {1} Caracteres")]
        public string Nombre { get; set; }

        [Display(Name ="Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; }

        public int UsuarioId { get; set; }
    }
}
