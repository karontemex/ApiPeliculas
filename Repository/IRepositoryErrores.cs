using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public interface IRepositoryErrores
    {
        Task Crear(Error error);
    }
}