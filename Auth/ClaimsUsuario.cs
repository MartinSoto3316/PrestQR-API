using System.Security.Claims;

namespace PrestQR_API.Auth
{
    /// <summary>Nombres de los claims del token y helpers para leerlos en los controladores.</summary>
    public static class ClaimsUsuario
    {
        public const string UsuarioId = "sub";
        public const string Email = "email";
        public const string Rol = "role";
        public const string AlumnoId = "alumno_id";

        public static int ObtenerUsuarioId(this ClaimsPrincipal usuario)
        {
            return int.Parse(usuario.FindFirstValue(UsuarioId)
                ?? throw new InvalidOperationException("El token no tiene el claim 'sub'."));
        }

        public static int? ObtenerAlumnoId(this ClaimsPrincipal usuario)
        {
            var valor = usuario.FindFirstValue(AlumnoId);
            return valor is null ? null : int.Parse(valor);
        }
    }
}
