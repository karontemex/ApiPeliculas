using ApiPeliculas;
using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using ApiPeliculas.Utilidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ApiPeliculas.Repository
{
    public class RepositoryPeliculas : IRepositoryPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;
        private readonly IMapper mapper;
        private readonly ILogger<RepositoryPeliculas> logger;

        public RepositoryPeliculas(ApplicationDbContext context, IHttpContextAccessor HttpContextAccessor, IMapper mapper, ILogger<RepositoryPeliculas> logger)
        {
            this.context = context;
            httpContext = HttpContextAccessor.HttpContext!;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<Pelicula>> GetPeliculas(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            return await queryable.OrderBy(static p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula> GetPelicula(int id)
        {
            return await context.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(p => p.ActoresPelicula.OrderBy(ap => ap.Orden))
                    .ThenInclude(ap => ap.Actor)
                .AsNoTracking()                
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Editar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }


        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {
            var pelicula = await context.Peliculas.Include(p => p.GenerosPelicula).FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula == null)
            {
                throw new ArgumentException($"No existe la pelicula con id {id}");
            }
            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula() { PeliculaId = id, GeneroId = generoId }).ToList();
            //Con este mapper se mapea 3 posioble sopciones enc ambios de generos, si existe uno y se elimina, si se agrega uno nuevo, o si se mantiene uno igual,
            //el mapper se encarga de todo eso, es decir, no se borra todo y se vuelve a agregar, sino que solo se hacen los cambios necesarios
            pelicula.GenerosPelicula = mapper.Map(generosPeliculas, pelicula.GenerosPelicula);
            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actoresPeliculas)
        {
            for (int i = 1; 1 <= actoresPeliculas.Count; i++) { 
                actoresPeliculas[i-1].Orden = i;
            }
            
            var pelicula = await context.Peliculas.Include(p => p.ActoresPelicula).FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula == null)
            {
                throw new ArgumentException($"No existe la pelicula con id {id}");
            }
            //Con este mapper se mapea 3 posibles opciones cambios de actores, si existe uno y se elimina, si se agrega uno nuevo, o si se mantiene uno igual,
            //el mapper se encarga de todo eso, es decir, no se borra todo y se vuelve a agregar, sino que solo se hacen los cambios necesarios
            pelicula.ActoresPelicula = mapper.Map(actoresPeliculas, pelicula.ActoresPelicula);
            await context.SaveChangesAsync();
        }

        public async Task<List<Pelicula>> Filtrar(PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();


            if (!string.IsNullOrWhiteSpace(peliculasFiltrarDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(peliculasFiltrarDTO.Titulo));
            }
            if (peliculasFiltrarDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.GenerosPelicula.Select(gp => gp.GeneroId).Contains(peliculasFiltrarDTO.GeneroId));
            }
            if (peliculasFiltrarDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCines);
            }
            if (peliculasFiltrarDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(p => p.FechaEstreno > hoy);
            }


            if (!string.IsNullOrWhiteSpace(peliculasFiltrarDTO.CampoOrdenar))
            {
                var tipoOrden = peliculasFiltrarDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{peliculasFiltrarDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                    throw new ArgumentException($"No se puede ordenar por el campo {peliculasFiltrarDTO.CampoOrdenar}");
                }
                
            }

            await httpContext.InsertarParametrosPaginacionEnCabecera(peliculasQueryable);
            var resultado = await peliculasQueryable.Paginar(peliculasFiltrarDTO.PaginacionDTO).ToListAsync();
            return resultado;
        }
}
}

