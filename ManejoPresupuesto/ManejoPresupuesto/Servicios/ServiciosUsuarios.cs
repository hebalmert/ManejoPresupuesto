namespace ManejoPresupuesto.Servicios
{
    public interface IServiciosUsuarios
    {
        int ObtenerUsuario();
    }


    public class ServiciosUsuarios : IServiciosUsuarios
    {
        public int ObtenerUsuario()
        {
            return 1;
        }
    }
}
