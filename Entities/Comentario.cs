using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Entities
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Cuerpo { get; set; } = null;
        public int PeliculaId { get; set; }

        public string UsuarioID { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;

    }
}
