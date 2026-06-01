namespace ApiPeliculas.DTOS
{
    public class CrearPeliculaDTO
    {
        public string Titulo { get; set; }
        public bool EnCines { get; set; } = false;
        public DateTime FechaEstreno { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
