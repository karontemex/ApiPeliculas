using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public interface IRepositoryActores
    {
        Task<List<Actor>> GetActores(PaginacionDTO paginacionDTO);
        Task<Actor?> GetActor(int id);
        Task<bool> Existe(int id);
        Task<int> Create(Actor actor);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetActoresPorNombre(string nombre);
        Task<List<int>> ExistenActores(List<int> idsActores);
    }
}
