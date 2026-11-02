using PrestQR_API.Models.DTOs;

namespace PrestQR_API.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UsuarioSesion> ObtenerSesionAsync(int usuarioId);
    }
}
