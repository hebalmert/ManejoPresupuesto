using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Defaultconnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                ($@"Insert Into Cuentas (Nombre, TiposCuentasId, Balance, Descripcion)
                Values(@Nombre, @TiposCuentasId, @Balance, @Descripcion);
                select SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;

        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(
                $@"Select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                   from Cuentas
                   inner join TiposCuentas tc
                   on tc.id = Cuentas.TiposCuentasId
                   where tc.UsuarioId = @UsuarioId
                   Order by tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                $@" Select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tc.Id
                    from Cuentas
                    inner join TiposCuentas tc
                    on tc.Id = Cuentas.TiposCuentasId
                    where tc.UsuarioId = @UsuarioId and Cuentas.Id = @Id", new { id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta) 
        {
            using var connection = new SqlConnection(connectionString);
            await connection.QueryAsync($@"Update Cuentas
                              Set Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TiposCuentasId = @TiposCuentasId
                              Where Id = @Id;", cuenta);
        }
    }
}
