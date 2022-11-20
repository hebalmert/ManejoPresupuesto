using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategoria
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
        Task<Categoria> ObtnerPorId(int id, int usuarioId);
    }

    public class RepositorioCategoria : IRepositorioCategoria
    {

        private readonly String connectionString;

        public RepositorioCategoria(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Defaultconnection");
        }


        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                ($@" Insert Into Categorias(Nombre, TipoOperacionId, UsuarioId)
                    values(@Nombre, @TipoOperacionId, @UsuarioId)
                    
                    Select SCOPE_IDENTITY();", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>
                ($@"Select * from Categorias Where UsuarioId = @usuarioId", new { usuarioId });
        }

        public async Task<Categoria> ObtnerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>
                ($@"  Select * from Categorias 
                Where Id = @Id and UsuarioId = @UsuarioId", new { id, usuarioId });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync
                ($@" Update Categorias
                Set Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                where Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync
                ($@" Delete Categorias where Id = @Id ", new { id});
        }
    }

}
