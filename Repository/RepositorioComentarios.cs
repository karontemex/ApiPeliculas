using ApiPeliculas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repository
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDbContext context;

        public RepositorioComentarios(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Comentario>> GetComentariosByPeliculaId(int peliculaId)
        {
            return await context.Comentarios.Where(c => c.PeliculaId == peliculaId).ToListAsync();
        }

        public async Task<Comentario?> ComentarioById(int id)
        {
            return await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> Crear(Comentario comentario)
        {
            //context.Comentarios.Add(comentario);
            context.Add(comentario);
            await context.SaveChangesAsync();
            return comentario.Id;
        }
        public async Task Actualizar(Comentario comentario)
        {
            context.Update(comentario);
            await context.SaveChangesAsync();
        }
        public async Task Eliminar(int id)
        {
            await context.Comentarios.Where(c => c.Id == id).ExecuteDeleteAsync();
        }
        public async Task<bool> Existe(int id)
        {
            return await context.Comentarios.AnyAsync(c => c.Id == id);
        }
    }
}

