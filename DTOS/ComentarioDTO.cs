namespace ApiPeliculas.DTOS
{
    public class ComentarioDTO
    {
        public int Id { get; set; }
        public string? Cuerpo { get; set; } = null;
        public string PeliculaId { get; set; }
    }
}