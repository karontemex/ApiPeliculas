using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public interface IRepositorioComentarios
    {
        Task Actualizar(Comentario comentario);
        Task<Comentario?> ComentarioById(int id);
        Task<int> Crear(Comentario comentario);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<List<Comentario>> GetComentariosByPeliculaId(int peliculaId);
    }
}