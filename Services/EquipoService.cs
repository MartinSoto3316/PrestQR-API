using PrestQR_API.Exceptions;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Repositories;

namespace PrestQR_API.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _equipos;
        private readonly IPrestamoRepository _prestamos;

        public EquipoService(IEquipoRepository equipos, IPrestamoRepository prestamos)
        {
            _equipos = equipos;
            _prestamos = prestamos;
        }

        public Task<IEnumerable<Equipo>> ListarAsync(string? estado)
        {
            if (estado is not null && !EstadosEquipo.Todos.Contains(estado))
            {
                throw new ValidacionException($"Estado inválido. Valores permitidos: {string.Join(", ", EstadosEquipo.Todos)}.");
            }

            return _equipos.ListarAsync(estado);
        }

        public async Task<Equipo> ObtenerAsync(int id)
        {
            return await _equipos.ObtenerPorIdAsync(id)
                ?? throw new NoEncontradoException($"El equipo {id} no existe.");
        }

        public async Task<EscaneoResponse> EscanearAsync(string codigoQr)
        {
            var codigo = NormalizarCodigo(codigoQr);
            var equipo = await _equipos.ObtenerPorCodigoQrAsync(codigo)
                ?? throw new NoEncontradoException($"No hay ningún equipo con el código {codigo}.");

            // Equipo disponible → solo se puede prestar.
            if (equipo.Estado == EstadosEquipo.Disponible)
            {
                return new EscaneoResponse { Equipo = equipo, AccionPermitida = EscaneoResponse.AccionPrestar };
            }

            // Equipo prestado → se muestra el préstamo activo y solo se puede devolver.
            var prestamoActivo = await _prestamos.ObtenerActivoPorEquipoAsync(equipo.Id);
            return new EscaneoResponse
            {
                Equipo = equipo,
                PrestamoActivo = prestamoActivo,
                AccionPermitida = EscaneoResponse.AccionDevolver
            };
        }

        public async Task<Equipo> CrearAsync(CrearEquipoRequest request)
        {
            var id = await _equipos.CrearAsync(NormalizarCodigo(request.CodigoQr), request.Nombre.Trim(), request.Modelo?.Trim());
            return await ObtenerAsync(id);
        }

        public async Task<Equipo> ActualizarAsync(int id, ActualizarEquipoRequest request)
        {
            var actualizado = await _equipos.ActualizarAsync(id, request.Nombre.Trim(), request.Modelo?.Trim());
            if (!actualizado)
            {
                throw new NoEncontradoException($"El equipo {id} no existe.");
            }

            return await ObtenerAsync(id);
        }

        public async Task EliminarAsync(int id)
        {
            // Un equipo con préstamos forma parte del historial y no se borra.
            if (await _equipos.TienePrestamosAsync(id))
            {
                throw new ReglaNegocioException("El equipo tiene préstamos registrados y no puede eliminarse.");
            }

            if (!await _equipos.EliminarAsync(id))
            {
                throw new NoEncontradoException($"El equipo {id} no existe.");
            }
        }

        // Los códigos se guardan en mayúsculas para que "lap-001" y "LAP-001" sean el mismo equipo.
        private static string NormalizarCodigo(string codigoQr) => codigoQr.Trim().ToUpperInvariant();
    }
}
