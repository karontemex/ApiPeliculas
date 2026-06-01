using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Entities
{
    public class Genero
    {
        public int Id { get; set; }

        //[StringLength(50)]
        public string Nombre { get; set; } = null!;
        public List<GeneroPelicula> GenerosPelicula { get; set; } = new List<GeneroPelicula>();
    }
}
