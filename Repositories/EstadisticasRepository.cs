using Dapper;
using Npgsql;
using PrestQR_API.Models.DTOs;

namespace PrestQR_API.Repositories
{
    public class EstadisticasRepository : IEstadisticasRepository
    {
        private const string HoyHermosillo = "(now() AT TIME ZONE 'America/Hermosillo')::date";
        // ::timestamp para comparar contra fechas locales de Hermosillo sin que intervenga la zona horaria de la sesión.
        private const string MesActual = $"date_trunc('month', {HoyHermosillo}::timestamp)";

        // Las cuatro consultas viajan juntas en una sola ida a la base de datos.
        private const string Consultas = $"""
            SELECT count(*)::int                                         AS total,
                   count(*) FILTER (WHERE estado = 'disponible')::int    AS disponibles,
                   count(*) FILTER (WHERE estado = 'prestado')::int      AS prestados
              FROM public.equipos;

            SELECT count(*) FILTER (WHERE estado = 'activo')::int                                        AS activos,
                   count(*) FILTER (WHERE estado = 'activo' AND fecha_limite <  {HoyHermosillo})::int   AS vencidos,
                   count(*) FILTER (WHERE estado = 'activo' AND fecha_limite =  {HoyHermosillo})::int   AS vencen_hoy,
                   count(*) FILTER (WHERE date_trunc('month', fecha_prestamo   AT TIME ZONE 'America/Hermosillo') = {MesActual})::int AS este_mes,
                   count(*) FILTER (WHERE date_trunc('month', fecha_devolucion AT TIME ZONE 'America/Hermosillo') = {MesActual})::int AS devueltos_este_mes
              FROM public.prestamos;

            SELECT e.id AS equipo_id, e.codigo_qr, e.nombre, count(*)::int AS veces
              FROM public.prestamos p
              JOIN public.equipos e ON e.id = p.equipo_id
             GROUP BY e.id, e.codigo_qr, e.nombre
             ORDER BY veces DESC, e.nombre
             LIMIT 5;

            SELECT to_char(m.mes, 'YYYY-MM') AS mes, count(p.id)::int AS total
              FROM generate_series({MesActual} - interval '5 months', {MesActual}, interval '1 month') AS m(mes)
              LEFT JOIN public.prestamos p
                     ON date_trunc('month', p.fecha_prestamo AT TIME ZONE 'America/Hermosillo') = m.mes
             GROUP BY m.mes
             ORDER BY m.mes;
            """;

        private readonly NpgsqlDataSource _dataSource;

        public EstadisticasRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<EstadisticasResponse> ObtenerAsync()
        {
            await using var conn = _dataSource.CreateConnection();
            using var resultados = await conn.QueryMultipleAsync(Consultas);

            return new EstadisticasResponse
            {
                Equipos = await resultados.ReadSingleAsync<ResumenEquipos>(),
                Prestamos = await resultados.ReadSingleAsync<ResumenPrestamos>(),
                EquiposMasPrestados = await resultados.ReadAsync<EquipoMasPrestado>(),
                PrestamosPorMes = await resultados.ReadAsync<PrestamosPorMes>()
            };
        }
    }
}
