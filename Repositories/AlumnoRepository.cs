using Dapper;
using Npgsql;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public class AlumnoRepository : IAlumnoRepository
    {
        private const string SelectAlumno = """
            SELECT a.id, a.usuario_id, a.matricula, a.nombre, a.carrera, u.email, u.activo
              FROM public.alumnos a
              JOIN public.usuarios u ON u.id = a.usuario_id
            """;

        private readonly NpgsqlDataSource _dataSource;

        public AlumnoRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Alumno>> ListarAsync(string? buscar)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QueryAsync<Alumno>(
                $"""
                {SelectAlumno}
                 WHERE (@Buscar IS NULL
                        OR a.nombre    ILIKE '%' || @Buscar || '%'
                        OR a.matricula ILIKE '%' || @Buscar || '%')
                 ORDER BY a.nombre
                """,
                new { Buscar = string.IsNullOrWhiteSpace(buscar) ? null : buscar.Trim() });
        }

        public async Task<Alumno?> ObtenerPorIdAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Alumno>(
                $"{SelectAlumno} WHERE a.id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(string email, string passwordHash, string matricula, string nombre, string? carrera)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                "SELECT public.fn_crear_alumno(@Email, @PasswordHash, @Matricula, @Nombre, @Carrera)",
                new { Email = email, PasswordHash = passwordHash, Matricula = matricula, Nombre = nombre, Carrera = carrera });
        }

        public async Task ActualizarAsync(int id, string email, string matricula, string nombre, string? carrera)
        {
            await using var conn = _dataSource.CreateConnection();
            await conn.ExecuteAsync(
                "SELECT public.fn_actualizar_alumno(@Id, @Email, @Matricula, @Nombre, @Carrera)",
                new { Id = id, Email = email, Matricula = matricula, Nombre = nombre, Carrera = carrera });
        }

        public async Task<bool> CambiarActivoAsync(int id, bool activo)
        {
            await using var conn = _dataSource.CreateConnection();
            var filas = await conn.ExecuteAsync(
                """
                UPDATE public.usuarios u
                   SET activo = @Activo
                  FROM public.alumnos a
                 WHERE a.usuario_id = u.id
                   AND a.id = @Id
                """,
                new { Id = id, Activo = activo });
            return filas > 0;
        }
    }
}
