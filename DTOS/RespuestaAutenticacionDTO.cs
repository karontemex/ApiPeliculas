namespace ApiPeliculas.DTOS
{
    public class RespuestaAutenticacionDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration {  get; set; }
    }
}
