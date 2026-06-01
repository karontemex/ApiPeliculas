namespace ApiPeliculas.Entities
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool EnCines { get; set; } = false;
        public DateTime FechaEstreno { get; set; }
        public string? Poster { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<GeneroPelicula> GenerosPelicula { get; set; } = new List<GeneroPelicula>();
        public List<ActorPelicula> ActoresPelicula { get; set;} = new List<ActorPelicula>();
    }
}
