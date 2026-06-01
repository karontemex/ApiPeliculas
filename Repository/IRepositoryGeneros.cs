using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public interface IRepositoryGeneros
    {
            Task<List<Genero>> GetGeneros();
            Task<Genero?> GetGeneroById(int id);
            Task<int> CreateGenero(Genero genero);
            Task UpdateGenero(Genero genero);
            Task DeleteGenero(int id);
            Task<List<int>> ExistenGeneros(List<int> generosIds);
            Task<bool> GeneroExists(int id);
            Task<bool> GeneroExists(int id, string nombre);
    }
}
