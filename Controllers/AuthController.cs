using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestQR_API.Auth;
using PrestQR_API.Models.DTOs;
using PrestQR_API.Services;

namespace PrestQR_API.Controllers
{
    /// <summary>
    /// Inicio de sesión con JWT. No hay endpoint de logout:
    /// cerrar sesión es borrar el token guardado en la PWA.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Valida email y contraseña y devuelve el token con los datos del usuario.</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            return Ok(await _authService.LoginAsync(request));
        }

        /// <summary>Usuario del token actual. La PWA lo usa al abrir para saber si la sesión sigue válida.</summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UsuarioSesion>> Me()
        {
            return Ok(await _authService.ObtenerSesionAsync(User.ObtenerUsuarioId()));
        }
    }
}
