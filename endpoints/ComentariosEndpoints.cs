using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using ApiPeliculas.Repository;
using Microsoft.AspNetCore.OutputCaching;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using ApiPeliculas.Filtros;
using ApiPeliculas.Services;

namespace ApiPeliculas.endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group) {
            group.MapGet("/", GetAll).WithName("GetAllComentarios").CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("copmentarios-tag"));
            group.MapGet("/{id:int}", Get).WithName("ObtenerporID");
            group.MapPost("/", Crear).WithName("CrearComentario").AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapPut("/{id:int}",Update).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapDelete("/{id:int}", Delete).RequireAuthorization();

            return group;
        }

        static async Task<Results<CreatedAtRoute<ComentarioDTO>, NotFound,BadRequest<string>>> Crear(int peliculaId, CrearComentarioDTO crearComentarioDTO, 
            IRepositorioComentarios repositorioComentarios, IRepositoryPeliculas repositoryPeliculas,
            IMapper mapper, IOutputCacheStore cacheStore, IServicioUsuarios servicioUsuarios)
        {
            if (!await repositoryPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var usuario = await servicioUsuarios.ObtenerUsuario();
            if (usuario is null) {
                return TypedResults.BadRequest("Usuario no encontrado");
            }
            comentario.UsuarioID = usuario.Id;
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

        static async Task<Results<NoContent, NotFound,ForbidHttpResult>> Update(int comentarioId, int paliculaId, CrearComentarioDTO crearComentarioDTO, 
            IRepositorioComentarios repositorioComentarios,IRepositoryPeliculas repositoryPeliculas, IOutputCacheStore cacheStore, IServicioUsuarios servicioUsuarios)
        {
            if(!await repositoryPeliculas.Existe(paliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentarioDB = await repositorioComentarios.ComentarioById(comentarioId);
            if (comentarioDB == null) {
                return TypedResults.NotFound();
            }
            var usuario = await servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.UsuarioID != usuario.Id) {
                return TypedResults.Forbid();
            }

            comentarioDB.Cuerpo = crearComentarioDTO.Cuerpo;
            await repositorioComentarios.Actualizar(comentarioDB);
            await cacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Delete(int comentarioId,int peliculaId, IRepositorioComentarios repositorioComentarios, IOutputCacheStore cacheStore, IServicioUsuarios servicioUsuarios)
        {
            var comentarioDB = await repositorioComentarios.ComentarioById(comentarioId);
            if (comentarioDB == null)
            {
                return TypedResults.NotFound();
            }
            var usuario = await servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.UsuarioID != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            await repositorioComentarios.Eliminar(comentarioId);
            return TypedResults.NoContent();
        }
    }
}
