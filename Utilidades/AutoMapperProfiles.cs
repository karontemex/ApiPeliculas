using ApiPeliculas.DTOS;
using ApiPeliculas.Entities;
using AutoMapper;

namespace ApiPeliculas.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDto>();

            CreateMap<CrearActorDto, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Actor, ActorDto>();

            CreateMap<CrearPeliculaDTO, Pelicula>()
                            .ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(y => y.GenerosPelicula.Select(gp => new GeneroDto { Id = gp.Genero.Id, Nombre = gp.Genero.Nombre })))
                .ForMember(x => x.Actores, options => options.MapFrom(y => y.ActoresPelicula.Select(ap => new ActorPeliculaDTO { Id = ap.Actor.Id, Nombre = ap.Actor.Nombre, Personaje = ap.Personaje })));

            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();
            CreateMap<ActorPelicula, AsignarActorPeliculaDTO>();
        }
    }
}
