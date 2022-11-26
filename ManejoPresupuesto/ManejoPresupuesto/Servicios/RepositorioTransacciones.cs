using AspNetCore;
using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
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


        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new 
                { 
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,   
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>
                ($@"Select Transacciones.*, cat.TipoOperacionId 
                 from Transacciones
                 inner join Categorias cat
                 on cat.Id = Transacciones.CategoriaId
                 Where Transacciones.Id = @Id and Transacciones.UsuarioId = @UsuarioId",
                 new { id , usuarioId});
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuenta(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                ($@"Select  t.Id, t.Monto, t.FechaTransaccion, C.Nombre as Categoria,
                 cu.Nombre as Cuenta, c.TipoOperacionId
                 from Transacciones t
                 inner join Categoria c
                 on c.Id = t.CategoriaId
                 inner join Cuentas cu
                 on cu.Id = t.CuentaId
                 Where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                 and FechaTransaccion Between @FechaInicio and @FechaFin", modelo);
        }



        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new 
            { 
                id
            },
            commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
