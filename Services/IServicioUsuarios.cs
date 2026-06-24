using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Services
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}