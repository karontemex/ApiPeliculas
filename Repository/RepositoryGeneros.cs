using ApiPeliculas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ApiPeliculas.Repository
{
    public class RepositoryGeneros : IRepositoryGeneros
    {
        private readonly ApplicationDbContext _context;
        public RepositoryGeneros(ApplicationDbContext context) { 
            this._context = context;
        }
        public async Task<List<Genero>> GetGeneros()
        {
            //return Task.FromResult(_context.Generos.ToList());
            
                return await _context.Generos.OrderBy(x=> x.Nombre).ToListAsync();
        }
        public async Task<Genero?> GetGeneroById(int id)
        {
            return await _context.Generos.FirstOrDefaultAsync(g => g.Id == id);
        }        
        public async Task<int> CreateGenero(Genero genero)
        {
            _context.Add(genero);
            await _context.SaveChangesAsync();
            return genero.Id;
        }
        public async Task DeleteGenero(int id)
        {
          await _context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();            
        }
        public async Task UpdateGenero(Genero genero)
        {
            _context.Update(genero);
            await _context.SaveChangesAsync();
            
        }
        public async Task<bool> GeneroExists(int id)
        {
            return await _context.Generos.AnyAsync(x => x.Id == id);
        }
        public async Task<bool> GeneroExists(int id, string nombre) {
            return await _context.Generos.AnyAsync(g => g.Id != id && g.Nombre == nombre);
        }
        public async Task<List<int>> ExistenGeneros(List<int> generosIds)
        {
            return await _context.Generos.Where(g => generosIds.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        }
    }
}
