using PrestQR_API.Exceptions;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Repositories;

namespace PrestQR_API.Services
{
    /// <summary>
    /// Las reglas de préstamo y devolución (equipo disponible, préstamo activo, fecha límite)
    /// se validan de forma atómica en fn_registrar_prestamo y fn_registrar_devolucion.
    /// Aquí se validan los filtros y, más adelante, se enviarán las notificaciones al alumno.
    /// </summary>
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamos;

        public PrestamoService(IPrestamoRepository prestamos)
        {
            _prestamos = prestamos;
        }

        public Task<IEnumerable<Prestamo>> ListarAsync(PrestamoFiltro filtro)
        {
            if (filtro.Estado is not null && !EstadosPrestamo.Todos.Contains(filtro.Estado))
            {
                throw new ValidacionException($"Estado inválido. Valores permitidos: {string.Join(", ", EstadosPrestamo.Todos)}.");
            }

            return _prestamos.ListarAsync(filtro);
        }

        public async Task<Prestamo> ObtenerAsync(int id)
        {
            return await _prestamos.ObtenerPorIdAsync(id)
                ?? throw new NoEncontradoException($"El préstamo {id} no existe.");
        }

        public async Task<Prestamo> RegistrarAsync(CrearPrestamoRequest request, int adminId)
        {
            var id = await _prestamos.RegistrarAsync(
                request.EquipoId!.Value,
                request.AlumnoId!.Value,
                adminId,
                request.FechaLimite!.Value,
                string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim());

            // TODO (notificaciones): avisar al alumno "Tu préstamo fue registrado…"
            return await ObtenerAsync(id);
        }

        public async Task<Prestamo> RegistrarDevolucionAsync(int id)
        {
            await _prestamos.RegistrarDevolucionAsync(id);

            // TODO (notificaciones): avisar al alumno "La devolución fue registrada…"
            return await ObtenerAsync(id);
        }
    }
}
