namespace ApiPeliculas.DTOS
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool EnCines { get; set; } = false;
        public DateTime FechaEstreno { get; set; }
        public string? Poster { get; set; }

        public List<ComentarioDTO> Comentarios { get; set; } = new List<ComentarioDTO>();
        public List<GeneroDto> Generos { get; set; } = new List<GeneroDto>();

        public List<ActorPeliculaDTO> Actores { get; set; } = new List<ActorPeliculaDTO>();
    }
}
