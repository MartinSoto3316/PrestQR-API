using Dapper;
using Npgsql;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public class PrestamoRepository : IPrestamoRepository
    {
        // "Vencido" no es un estado guardado: es un préstamo activo cuya fecha límite ya pasó en Hermosillo.
        private const string HoyHermosillo = "(now() AT TIME ZONE 'America/Hermosillo')::date";

        private const string SelectPrestamo = $"""
            SELECT p.id,
                   p.equipo_id,  e.codigo_qr AS equipo_codigo_qr, e.nombre AS equipo_nombre,
                   p.alumno_id,  a.matricula AS alumno_matricula, a.nombre AS alumno_nombre,
                   p.admin_id,
                   p.fecha_prestamo, p.fecha_limite, p.fecha_devolucion,
                   p.estado, p.observaciones,
                   (p.estado = 'activo' AND p.fecha_limite < {HoyHermosillo}) AS vencido
              FROM public.prestamos p
              JOIN public.equipos e ON e.id = p.equipo_id
              JOIN public.alumnos a ON a.id = p.alumno_id
            """;

        private readonly NpgsqlDataSource _dataSource;

        public PrestamoRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Prestamo>> ListarAsync(PrestamoFiltro filtro)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QueryAsync<Prestamo>(
                $"""
                {SelectPrestamo}
                 WHERE (@Estado   IS NULL OR p.estado    = @Estado)
                   AND (@EquipoId IS NULL OR p.equipo_id = @EquipoId)
                   AND (@AlumnoId IS NULL OR p.alumno_id = @AlumnoId)
                   AND (@Vencidos = false OR (p.estado = 'activo' AND p.fecha_limite < {HoyHermosillo}))
                 ORDER BY p.fecha_prestamo DESC
                """,
                new { filtro.Estado, filtro.EquipoId, filtro.AlumnoId, filtro.Vencidos });
        }

        public async Task<Prestamo?> ObtenerPorIdAsync(int id)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Prestamo>(
                $"{SelectPrestamo} WHERE p.id = @Id",
                new { Id = id });
        }

        public async Task<Prestamo?> ObtenerActivoPorEquipoAsync(int equipoId)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Prestamo>(
                $"{SelectPrestamo} WHERE p.equipo_id = @EquipoId AND p.estado = 'activo'",
                new { EquipoId = equipoId });
        }

        public async Task<int> RegistrarAsync(int equipoId, int alumnoId, int adminId, DateOnly fechaLimite, string? observaciones)
        {
            await using var conn = _dataSource.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                "SELECT public.fn_registrar_prestamo(@EquipoId, @AlumnoId, @AdminId, @FechaLimite, @Observaciones)",
                new
                {
                    EquipoId = equipoId,
                    AlumnoId = alumnoId,
                    AdminId = adminId,
                    FechaLimite = fechaLimite,
                    Observaciones = observaciones
                });
        }

        public async Task RegistrarDevolucionAsync(int prestamoId)
        {
            await using var conn = _dataSource.CreateConnection();
            await conn.ExecuteAsync(
                "SELECT public.fn_registrar_devolucion(@PrestamoId)",
                new { PrestamoId = prestamoId });
        }
    }
}
