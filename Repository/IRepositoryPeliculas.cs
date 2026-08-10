using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public interface IRepositoryPeliculas
    {
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task Editar(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula> GetPelicula(int id);
        Task<List<Pelicula>> GetPeliculas(PaginacionDTO paginacionDTO);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task AsignarActores(int id, List<ActorPelicula> actoresPeliculas);
        Task <List<Pelicula>> Filtrar(PeliculasFiltrarDTO peliculasFiltrarDTO);
    }
}