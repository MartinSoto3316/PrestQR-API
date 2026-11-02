using Dapper;
using Npgsql;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        // LEFT JOIN: los administradores no tienen fila en alumnos.
        private const string SelectUsuario = """
            SELECT u.id, u.email, u.password_hash, u.rol, u.activo,
                   a.id     AS alumno_id,
                   a.nombre AS alumno_nombre
              FROM public.usuarios u
              LEFT JOIN public.alumnos a ON a.usuario_id = u.id
            """;

        private readonly NpgsqlDataSource _dataSource;

        public UsuarioRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Usuario>(
                $"{SelectUsuario} WHERE lower(u.email) = lower(@Email)",
                new { Email = email.Trim() });
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Usuario>(
                $"{SelectUsuario} WHERE u.id = @Id",
                new { Id = id });
        }
    }
}
