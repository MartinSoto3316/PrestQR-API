using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PrestQR_API.Auth;
using PrestQR_API.Exceptions;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Models.Entities;
using PrestQR_API.Repositories;

namespace PrestQR_API.Services
{
    public class AuthService : IAuthService
    {
        // Mismo mensaje si el email no existe o la contraseña es incorrecta,
        // para no revelar qué emails están registrados.
        private const string CredencialesInvalidas = "Email o contraseña incorrectos.";

        private readonly IUsuarioRepository _usuarios;
        private readonly JwtOpciones _jwt;

        public AuthService(IUsuarioRepository usuarios, IOptions<JwtOpciones> jwt)
        {
            _usuarios = usuarios;
            _jwt = jwt.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var usuario = await _usuarios.ObtenerPorEmailAsync(request.Email);

            if (usuario is null || !PasswordCorrecta(request.Password, usuario.PasswordHash))
            {
                throw new NoAutorizadoException(CredencialesInvalidas);
            }

            if (!usuario.Activo)
            {
                throw new NoAutorizadoException("Tu cuenta está desactivada. Acude con el administrador.");
            }

            var expiraEn = DateTime.UtcNow.AddMinutes(_jwt.MinutosExpiracion);

            return new LoginResponse
            {
                Token = GenerarToken(usuario, expiraEn),
                ExpiraEn = expiraEn,
                Usuario = ASesion(usuario)
            };
        }

        public async Task<UsuarioSesion> ObtenerSesionAsync(int usuarioId)
        {
            var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId);

            // El token puede seguir vigente aunque la cuenta se haya desactivado después.
            if (usuario is null || !usuario.Activo)
            {
                throw new NoAutorizadoException("La sesión ya no es válida. Inicia sesión de nuevo.");
            }

            return ASesion(usuario);
        }

        // Un hash mal formado (por ejemplo, un texto puesto a mano en la BD) hace que BCrypt lance excepción;
        // se trata como contraseña incorrecta en vez de responder 500.
        private static bool PasswordCorrecta(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }

        private string GenerarToken(Usuario usuario, DateTime expiraEn)
        {
            var claims = new List<Claim>
            {
                new(ClaimsUsuario.UsuarioId, usuario.Id.ToString()),
                new(ClaimsUsuario.Email, usuario.Email),
                new(ClaimsUsuario.Rol, usuario.Rol)
            };

            if (usuario.AlumnoId is int alumnoId)
            {
                claims.Add(new Claim(ClaimsUsuario.AlumnoId, alumnoId.ToString()));
            }

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave));

            return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwt.Emisor,
                Audience = _jwt.Audiencia,
                Expires = expiraEn,
                SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256)
            });
        }

        private static UsuarioSesion ASesion(Usuario usuario) => new()
        {
            UsuarioId = usuario.Id,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Nombre = usuario.AlumnoNombre ?? usuario.Email,
            AlumnoId = usuario.AlumnoId
        };
    }
}
