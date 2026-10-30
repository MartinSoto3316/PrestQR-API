using Dapper;
using Npgsql;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public class EquipoRepository : IEquipoRepository
    {
        private const string SelectEquipo = """
            SELECT id, codigo_qr, nombre, modelo, estado, creado_en
              FROM public.equipos
            """;

        private readonly NpgsqlDataSource _dataSource;

        public EquipoRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Equipo>> ListarAsync(string? estado)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QueryAsync<Equipo>(
                $"""
                {SelectEquipo}
                 WHERE (@Estado IS NULL OR estado = @Estado)
                 ORDER BY codigo_qr
                """,
                new { Estado = estado });
        }

        public async Task<Equipo?> ObtenerPorIdAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Equipo>(
                $"{SelectEquipo} WHERE id = @Id",
                new { Id = id });
        }

        public async Task<Equipo?> ObtenerPorCodigoQrAsync(string codigoQr)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Equipo>(
                $"{SelectEquipo} WHERE codigo_qr = @CodigoQr",
                new { CodigoQr = codigoQr });
        }

        public async Task<int> CrearAsync(string codigoQr, string nombre, string? modelo)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                """
                INSERT INTO public.equipos (codigo_qr, nombre, modelo)
                VALUES (@CodigoQr, @Nombre, @Modelo)
                RETURNING id
                """,
                new { CodigoQr = codigoQr, Nombre = nombre, Modelo = modelo });
        }

        public async Task<bool> ActualizarAsync(int id, string nombre, string? modelo)
        {
            await using var conn = _dataSource.CreateConnection();
            var filas = await conn.ExecuteAsync(
                """
                UPDATE public.equipos
                   SET nombre = @Nombre,
                       modelo = @Modelo
                 WHERE id = @Id
                """,
                new { Id = id, Nombre = nombre, Modelo = modelo });
            return filas > 0;
        }

        public async Task<bool> TienePrestamosAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.ExecuteScalarAsync<bool>(
                "SELECT EXISTS (SELECT 1 FROM public.prestamos WHERE equipo_id = @Id)",
                new { Id = id });
        }

        public async Task<bool> EliminarAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            var filas = await conn.ExecuteAsync(
                "DELETE FROM public.equipos WHERE id = @Id",
                new { Id = id });
            return filas > 0;
        }
    }
}
