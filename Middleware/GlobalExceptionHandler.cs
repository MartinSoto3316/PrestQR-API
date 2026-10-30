using Microsoft.AspNetCore.Diagnostics;
using Npgsql;
using PrestQR_API.Exceptions;

namespace PrestQR_API.Middleware
{
    /// <summary>
    /// Convierte las excepciones de la API y de PostgreSQL en respuestas HTTP
    /// con formato ProblemDetails, para que el cliente reciba 400/404/409 en vez de 500.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, mensaje) = exception switch
            {
                NoEncontradoException e => (StatusCodes.Status404NotFound, e.Message),
                ReglaNegocioException e => (StatusCodes.Status409Conflict, e.Message),
                ValidacionException e => (StatusCodes.Status400BadRequest, e.Message),
                PostgresException e => TraducirPostgres(e),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Error no controlado en {Ruta}", httpContext.Request.Path);
            }

            httpContext.Response.StatusCode = status;
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails =
                {
                    Status = status,
                    Title = TituloPara(status),
                    Detail = mensaje
                }
            });
        }

        private static (int, string) TraducirPostgres(PostgresException e) => e.SqlState switch
        {
            // Códigos propios lanzados por las funciones de Database/funciones.sql
            "PQ400" => (StatusCodes.Status400BadRequest, e.MessageText),
            "PQ404" => (StatusCodes.Status404NotFound, e.MessageText),
            "PQ409" => (StatusCodes.Status409Conflict, e.MessageText),

            PostgresErrorCodes.UniqueViolation => (StatusCodes.Status409Conflict, e.ConstraintName switch
            {
                "usuarios_email_key" => "El email ya está registrado.",
                "alumnos_matricula_key" => "La matrícula ya está registrada.",
                "equipos_codigo_qr_key" => "Ya existe un equipo con ese código QR.",
                "uq_prestamo_activo_por_equipo" => "El equipo ya tiene un préstamo activo.",
                _ => "El registro ya existe."
            }),

            PostgresErrorCodes.ForeignKeyViolation => (StatusCodes.Status409Conflict, e.ConstraintName switch
            {
                "fk_prestamos_admin" => "El administrador indicado no existe o no tiene rol admin.",
                "prestamos_equipo_id_fkey" => "El equipo tiene préstamos registrados y no puede eliminarse.",
                _ => "La operación hace referencia a un registro inexistente o que tiene historial."
            }),

            PostgresErrorCodes.CheckViolation => (StatusCodes.Status400BadRequest, e.ConstraintName switch
            {
                "ck_fecha_limite" => "La fecha límite no puede ser anterior a la fecha del préstamo.",
                "ck_fecha_devolucion" => "La fecha de devolución no puede ser anterior a la fecha del préstamo.",
                _ => "Uno de los valores no cumple las reglas del sistema."
            }),

            PostgresErrorCodes.StringDataRightTruncation => (StatusCodes.Status400BadRequest, "Uno de los campos excede la longitud permitida."),

            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error en la base de datos.")
        };

        private static string TituloPara(int status) => status switch
        {
            StatusCodes.Status400BadRequest => "Solicitud inválida",
            StatusCodes.Status404NotFound => "No encontrado",
            StatusCodes.Status409Conflict => "Conflicto",
            _ => "Error del servidor"
        };
    }
}
