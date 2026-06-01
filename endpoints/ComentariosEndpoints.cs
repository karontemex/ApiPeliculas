using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using ApiPeliculas.Repository;
using Microsoft.AspNetCore.OutputCaching;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using ApiPeliculas.Filtros;

namespace ApiPeliculas.endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group) {
            group.MapGet("/", GetAll).WithName("GetAllComentarios").CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("copmentarios-tag"));
            group.MapGet("/{id:int}", Get).WithName("ObtenerporID");
            group.MapPost("/", Crear).WithName("CrearComentario").AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>();
            group.MapPut("/{id:int}",Update).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>();
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Results<CreatedAtRoute<ComentarioDTO>, NotFound>> Crear(int peliculaId, CrearComentarioDTO crearComentarioDTO, 
            IRepositorioComentarios repositorioComentarios, IRepositoryPeliculas repositoryPeliculas,
            IMapper mapper, IOutputCacheStore cacheStore)
        {
            if (!await repositoryPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await cacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.CreatedAtRoute(comentarioDTO, "ObtenerporID", new { id , peliculaId });
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> GetAll(int peliculaId, IRepositorioComentarios repositorioComentarios, IRepositoryPeliculas repositoryPeliculas, IMapper mapper)
        {
            if (!await repositoryPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentarios = await repositorioComentarios.GetComentariosByPeliculaId(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);        
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> Get(int comentarioId, IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {

            var comentario = await repositorioComentarios.ComentarioById(comentarioId);
            if (comentario == null)
            {
                return TypedResults.NotFound();
            }
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int comentarioId, int paliculaId, CrearComentarioDTO crearComentarioDTO, 
            IRepositorioComentarios repositorioComentarios,IRepositoryPeliculas repositoryPeliculas, IMapper mapper, IOutputCacheStore cacheStore)
        {
            if(!await repositoryPeliculas.Existe(paliculaId))
            {
                return TypedResults.NotFound();
            }

            if (!await repositorioComentarios.Existe(comentarioId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.Id = comentarioId;
            comentario.PeliculaId = paliculaId;
            await repositorioComentarios.Actualizar(comentario);
            await cacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int comentarioId,int peliculaId, IRepositorioComentarios repositorioComentarios, IOutputCacheStore cacheStore)
        {
            var existe = await repositorioComentarios.Existe(comentarioId);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            await repositorioComentarios.Eliminar(comentarioId);
            return TypedResults.NoContent();
        }
    }
}
