namespace ApiPeliculas.Entities
{
    public class Error
    {  
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = null!; 
        public string? StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}
