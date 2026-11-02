using PrestQR_API.Models.Entities;

namespace PrestQR_API.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorIdAsync(int id);
    }
}
