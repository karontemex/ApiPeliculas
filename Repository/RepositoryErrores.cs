using ApiPeliculas.Entities;

namespace ApiPeliculas.Repository
{
    public class RepositoryErrores : IRepositoryErrores
    {
        private readonly ApplicationDbContext context;

        public RepositoryErrores(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Crear(Error error)
        {
            context.Errores.Add(error);
            await context.SaveChangesAsync();
        }
    }
}
