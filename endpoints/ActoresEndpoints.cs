using ApiPeliculas.DTOS;
using ApiPeliculas.Repository;
using ApiPeliculas.Services;
using ApiPeliculas.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Runtime.CompilerServices;
using FluentValidation;
using ApiPeliculas.Validaciones;
using ApiPeliculas.Filtros;

namespace ApiPeliculas.endpoints
{
    public static class ActoresEndpoints
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActores).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("actores-get"));
            group.MapGet("/{id:int}", GetActorById);
            group.MapGet("/buscarPorNombre/{nombre}", GetActoresPorNombre);
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDto>>().RequireAuthorization("esadmin");
            group.MapPut("/{id:int}", Update).DisableAntiforgery().RequireAuthorization("esadmin");
            group.MapDelete("/{id:int}", Delete).RequireAuthorization("esadmin");

            return group;
        }
        static async Task<Results<Created<ActorDto>,ValidationProblem>> Create([FromForm] CrearActorDto createActorDTO, IRepositoryActores repository, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            
            var actor = mapper.Map<Actor>(createActorDTO);
            if (createActorDTO.Foto != null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, createActorDTO.Foto);
                actor.Foto = url;
            }

            var id = await repository.Create(actor);

            await outputCacheStore.EvictByTagAsync("actores-get", default);

            var actorCreado = mapper.Map<ActorDto>(actor);
            return TypedResults.Created($"/actores/{id}", actorCreado);
        }
        static async Task<Ok<List<ActorDto>>> GetActores(IRepositoryActores repository, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO() { Pagina = pagina, RecordsPorPagina = recordsPorPagina };

            var actores = await repository.GetActores(paginacion);
            var actoresDTO = mapper.Map<List<ActorDto>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Ok<ActorDto>, NotFound>> GetActorById(int id, IRepositoryActores repository, IMapper mapper)
        {
            var actor = await repository.GetActor(id);
            if (actor == null)
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<ActorDto>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CrearActorDto updateActorDTO, IRepositoryActores repository, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore)
        {
            var actor = await repository.GetActor(id);
            if (actor == null)
            {
                return TypedResults.NotFound();
            }

            //mapper.Map(updateActorDTO, actor);

            var ActorDTO = mapper.Map<Actor>(updateActorDTO);
            ActorDTO.Foto = actor.Foto;
            ActorDTO.Id = id;
            if (updateActorDTO.Foto != null)
            {
                var url = await almacenadorArchivos.Editar(ActorDTO.Foto, contenedor, updateActorDTO.Foto);
                ActorDTO.Foto = url;
            }
            await repository.Update(ActorDTO);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IRepositoryActores repository, IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var actor = await repository.GetActor(id);

            if (actor == null)
            {
                return TypedResults.NotFound();
            }
            await repository.Delete(id);
            await almacenadorArchivos.Borrar(actor.Foto, contenedor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Ok<List<ActorDto>>> GetActoresPorNombre(string nombre, IRepositoryActores repository, IMapper mapper)
        {
            var actores = await repository.GetActoresPorNombre(nombre);
            var actoresDTO = mapper.Map<List<ActorDto>>(actores);
            return TypedResults.Ok(actoresDTO);
        }
    }

}
