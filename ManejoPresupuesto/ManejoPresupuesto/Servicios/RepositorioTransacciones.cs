using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
    }


    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly String connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Defaultconnection");
        }


        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                ($@"Transaccion_Insertar", new 
                { 
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

    }
}
