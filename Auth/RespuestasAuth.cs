using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace PrestQR_API.Auth
{
    /// <summary>
    /// Respuestas 401 y 403 con formato ProblemDetails y mensaje en español,
    /// igual que el resto de los errores de la API (por defecto llegan vacías).
    /// </summary>
    public static class RespuestasAuth
    {
        public static JwtBearerEvents Eventos() => new()
        {
            // 401: no hay token, es inválido o ya expiró.
            OnChallenge = async context =>
            {
                context.HandleResponse();

                var detalle = context.AuthenticateFailure switch
                {
                    SecurityTokenExpiredException => "Tu sesión expiró. Inicia sesión de nuevo.",
                    not null => "El token no es válido. Inicia sesión de nuevo.",
                    null => "Debes iniciar sesión."
                };

                await EscribirAsync(context.HttpContext, StatusCodes.Status401Unauthorized, "No autorizado", detalle);
            },

            // 403: el token es válido, pero el rol no tiene permiso.
            OnForbidden = context => EscribirAsync(
                context.HttpContext, StatusCodes.Status403Forbidden, "Prohibido", "No tienes permiso para realizar esta acción.")
        };

        private static async Task EscribirAsync(HttpContext httpContext, int status, string titulo, string detalle)
        {
            httpContext.Response.StatusCode = status;
            await httpContext.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Status = status,
                    Title = titulo,
                    Detail = detalle
                }
            });
        }
    }
}
