using ApiPeliculas.DTOS;
using ApiPeliculas.Repository;
using ApiPeliculas.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using FluentValidation;
using ApiPeliculas.Filtros;

namespace ApiPeliculas.endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("generos-get")).RequireAuthorization();

            group.MapGet("/{id:int}", GetGeneroById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30))).AddEndpointFilter<FiltroPrueba>();

            //group.MapPost("/", crearGenero).AddEndpointFilter<FiltroValidacionesGeneros>();//Aqui se usda un filtro de validación para validar el DTO, si no es válido se retorna un 400 con los errores de validación

            group.MapPost("/", crearGenero).AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("esadmin"); //Aqui se usda un filtro de validación generico

            group.MapPut("/{id:int}", actualizarGenero).RequireAuthorization("esadmin")
                .WithOpenApi(opciones => {
                    opciones.Summary = "Actualizar un género";
                    opciones.Description = "Podemos actualizar un genero";
                    opciones.Parameters[0].Description = "Id del género a actualizar";
                    opciones.RequestBody.Description = "El género que se quiere actualizar";

                    return opciones;
                });  //Aqui no se usa el filtro de validación porque queremos validar el DTO dentro del endpoint para poder retornar un 400 con los errores de validación, si lo hacemos con el filtro no podremos retornar un 400 y se lanzará una excepción

            group.MapDelete("/{id:int}", eliminarGenero).RequireAuthorization("esadmin"); 

            return group;
        }
        static async Task<Ok<List<GeneroDto>>> GetGeneros(IRepositoryGeneros repository, IMapper mapper, ILoggerFactory loggerFactory)
        {

            var tipo = typeof(GenerosEndpoints);

            var logger = loggerFactory.CreateLogger(tipo.FullName!);

            logger.LogInformation("Prueba logger - obteniendo lsitado de generos");

            var generos = await repository.GetGeneros();

            var generosDTO = mapper.Map<List<GeneroDto>>(generos);

            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDto>, NotFound>> GetGeneroById([AsParameters] ObtenerGeneroPorIDPeticionDTO modelo)
        {
            var genero = await modelo.GetGeneroById(modelo.id);
            if (genero == null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = modelo.mapper.Map<GeneroDto>(genero);

            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Created<GeneroDto>,ValidationProblem>> crearGenero(CrearGeneroDTO generoDTO, 
            IRepositoryGeneros repository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            //Se valida el DTO con el filtro de validación, si no es válido se retorna un 400 con los errores de validación
            var genero = mapper.Map<Genero>(generoDTO);

            var id = await repository.CreateGenero(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            var generoCreado = mapper.Map<GeneroDto>(genero);

            return TypedResults.Created($"/Generos/{id}", generoCreado);
        }

        static async Task<Results<NoContent, ValidationProblem, NotFound>> actualizarGenero(int id, CrearGeneroDTO genero, IRepositoryGeneros repository, 
            IOutputCacheStore outputCacheStore, IMapper mapper, IValidator<CrearGeneroDTO> validator)
        {
            //Validamos en el endpoint sin el filtro para poder retornar un 400 con los errores de validación, si lo hacemos con el filtro no podremos retornar un 400 y se lanzará una excepción
            var resultado = await validator.ValidateAsync(genero);

            if (!resultado.IsValid)
            {
                return TypedResults.ValidationProblem(resultado.ToDictionary());
            }

            var existeGenero = await repository.GeneroExists(id);
            if (!existeGenero)
            {
                return TypedResults.NotFound();
            }

            var generoEntity = mapper.Map<Genero>(genero);
            await repository.UpdateGenero(generoEntity);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> eliminarGenero(int id, IRepositoryGeneros repository, IOutputCacheStore outputCacheStore)
        {
            var existeGenero = await repository.GeneroExists(id);
            if (!existeGenero)
            {
                return TypedResults.NotFound();
            }
            await repository.DeleteGenero(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
