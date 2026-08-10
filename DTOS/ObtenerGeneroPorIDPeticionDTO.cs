using ApiPeliculas.Repository;
using AutoMapper;

namespace ApiPeliculas.DTOS
{
    public class ObtenerGeneroPorIDPeticionDTO
    {

        public int id {  get; set; }
        public IRepositoryGeneros repository { get; set; }
        public IMapper mapper { get; set; }
    }
}
