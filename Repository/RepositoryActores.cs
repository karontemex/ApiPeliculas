using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using ApiPeliculas.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repository
{
    public class RepositoryActores : IRepositoryActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;
        public RepositoryActores(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async Task<List<Actor>> GetActores(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsNoTracking().AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            return await queryable.OrderBy(a => a.Nombre).Paginar(paginacionDTO).ToListAsync();
        }
        public async Task<Actor?> GetActor(int id)
        {
            return await context.Actores.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(a => a.Id == id);
        }

        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();

            return actor.Id;
        }
        public async Task Update(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            /*
            var actor = await GetActor(id);
            if (actor != null) {
                context.Remove(actor);
                await context.SaveChangesAsync();
            }
            */
            await context.Actores.Where(a => a.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Actor>> GetActoresPorNombre(string nombre)
        {
            return await context.Actores.Where(a => a.Nombre.Contains(nombre)).OrderBy(a => a.Nombre).ToListAsync();
        }

        public async Task<List<int>> ExistenActores(List<int> idsActores)
        {
            return await context.Actores.Where(a => idsActores.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        }
    }
}

