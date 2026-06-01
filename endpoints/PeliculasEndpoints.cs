using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using ApiPeliculas.Filtros;
using ApiPeliculas.Repository;
using ApiPeliculas.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ApiPeliculas.endpoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string contenedor = "peliculas";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            // group.MapPost("/", Crear).WithName("CrearPelicula").DisableAntiforgery().Accepts<CrearPeliculaDTO>("multipart/form-data").Produces<PeliculaDTO>(StatusCodes.Status201Created);
            group.MapGet("/", GetPeliculas).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("peliculas-get"));
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>();
            group.MapGet("/{id:int}", GetPeliculaById);
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>();
            group.MapDelete("/{id:int}", Borrar).DisableAntiforgery();
            group.MapPost("/{id:int}/AsignarGeneros", AsignarGeneros).DisableAntiforgery();
            group.MapPost("/{id:int}/AsignarActores", AsignarActores).DisableAntiforgery();
            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositoryPeliculas repositoryPeliculas, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            // Aquí podrías crear una nueva película en tu base de datos o cualquier otra fuente de datos
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositoryPeliculas.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync($"peliculas-get", default);
            var peliculaCreada = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaCreada);
        }

        static async Task<Ok<List<PeliculaDTO>>> GetPeliculas(IRepositoryPeliculas repositoryPeliculas, IMapper mapper, int pagina = 1, int cantidad = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = cantidad };
            var peliculas = await repositoryPeliculas.GetPeliculas(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> GetPeliculaById(int id, IRepositoryPeliculas repositoryPeliculas, IMapper mapper)
        {
            var pelicula = await repositoryPeliculas.GetPelicula(id);

            if (pelicula == null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }



        static async Task<Results<NoContent, NotFound>> Actualizar(int id, IRepositoryPeliculas repositoryPeliculas, IMapper mapper, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositoryPeliculas.GetPelicula(id);

            if (peliculaDB == null)
            {
                return TypedResults.NotFound();
            }

            var peliculaActualizada = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaActualizada.Id = id;

            peliculaActualizada.Poster = peliculaDB.Poster;
            if (crearPeliculaDTO.Poster is not null)
            {
                peliculaActualizada.Poster = await almacenadorArchivos.Editar(peliculaDB.Poster, contenedor, crearPeliculaDTO.Poster);
            }
            await repositoryPeliculas.Editar(peliculaActualizada);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositoryPeliculas repositoryPeliculas, IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositoryPeliculas.GetPelicula(id);
            if (peliculaDB == null)
            {
                return TypedResults.NotFound();
            }

            if (peliculaDB.Poster is not null)
            {
                await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            }
            await outputCacheStore.EvictByTagAsync($"peliculas-get", default);

            await repositoryPeliculas.Borrar(id);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds,
            IRepositoryPeliculas repositoryPeliculas, IRepositoryGeneros repositoryGeneros)
        {
            if (!await repositoryPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }
            var generosExistente = new List<int>();

            if (generosIds.Count > 0)
            {
                generosExistente = await repositoryGeneros.ExistenGeneros(generosIds);
            }

            if (generosExistente.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistente);
                return TypedResults.BadRequest($"Los siguientes generos no existen: {string.Join(", ", generosNoExistentes)}");
            }

            await repositoryPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, BadRequest<string>, NotFound>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresPeliculas,
            IRepositoryPeliculas repositoryPeliculas, IRepositoryActores repositoryActores, IMapper mapper)
        {
            if (!await repositoryPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresPeliculas.Select(a => a.ActorId).ToList();

            if (actoresPeliculas.Count > 0)
            {
                actoresExistentes = await repositoryActores.ExistenActores(actoresIds);
            }

            if (actoresExistentes.Count != actoresPeliculas.Count)
            {
                var actoresNoExistentes = actoresPeliculas.Select(a => a.ActorId).Except(actoresExistentes);
                return TypedResults.BadRequest($"Los siguientes actores no existen: {string.Join(", ", actoresNoExistentes)}");
            }

            var actoresPeliculasResultado = mapper.Map<List<ActorPelicula>>(actoresPeliculas);
            await repositoryPeliculas.AsignarActores(id, actoresPeliculasResultado);
            return TypedResults.NoContent();
        }
    }
}
