using Dapper;
using ManejoPresupuesto.Controllers;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Threading.Tasks.Sources;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipocuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Defaultconnection");
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>
                ($@"Select Id, Nombre, Orden
                    from TiposCuentas
                    where usuarioId = @usuarioId
                    Order by Orden", new { usuarioId });
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                //Este era el procedimiento para Orden = 0 pero ahora para tomar el Orden en Automatico
                                //($@"INSERT INTO TiposCuentas
                                //(Nombre, UsuarioId, Orden)
                                //Values(@Nombre, @UsuarioId, 0);
                                //Select SCOPE_IDENTITY();", tipoCuenta);
                ($@"TiposCuentas_Insertar", new { UsuarioId = tipoCuenta.UsuarioId, Nombre = tipoCuenta.Nombre },
                commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;

        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync
                ($@"Update TiposCuentas
                set Nombre = @Nombre
                 where Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>
                     ($@"Select Id, Nombre, Orden
                     from TiposCuentas
                     where Id = @Id and UsuarioId = @UsuarioId", new { id, usuarioId });
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>
                ($@"SELECT 1
                FROM TiposCuentas
                WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId",
                new { nombre, usuarioId });

            return existe == 1;
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync($@"DELETE TiposCuentas Where Id = @Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipocuentasOrdenados)
        {
            var query = "Update TiposCuentas Set Orden = @Orden Where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipocuentasOrdenados);
        }
    }
}
