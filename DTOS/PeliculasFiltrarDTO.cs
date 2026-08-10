using ApiPeliculas.Utilidades;

namespace ApiPeliculas.DTOS
{
    public class PeliculasFiltrarDTO
    {
        public int Pagina { get; set; }
        public int RecordsPorPagina { get; set; } = 0;
        public PaginacionDTO PaginacionDTO
        {
            get {
                return new PaginacionDTO() {
                    Pagina = Pagina,
                    RecordsPorPagina = RecordsPorPagina,
                };
            }
        }  

        public string? Titulo { get; set; } = null;
        public int GeneroId { get; set; } = 0;
        public bool EnCines { get; set; } = false;
        public bool ProximosEstrenos { get; set; } = false;
        public string? CampoOrdenar { get; set; } = null;
        public bool OrdenAscendente { get; set; } = true;

        public static ValueTask<PeliculasFiltrarDTO> BindAsync(HttpContext context)
        {
            var pagina = context.ExatrerValorODefecto(nameof(Pagina), 1);
            var recordsPorPagina = context.ExatrerValorODefecto(nameof(RecordsPorPagina), 10);
            var titulo = context.ExatrerValorODefecto(nameof(Titulo), String.Empty);
            var generoId = context.ExatrerValorODefecto(nameof(GeneroId), 0);
            var enCines = context.ExatrerValorODefecto(nameof(EnCines), false);
            var proximosEstrenos = context.ExatrerValorODefecto(nameof(ProximosEstrenos), false);
            var campoOrdenar = context.ExatrerValorODefecto(nameof(CampoOrdenar), String.Empty);
            var ordenAscendente = context.ExatrerValorODefecto(nameof(OrdenAscendente), true);

            
            var peliculasFiltrarDTO = new PeliculasFiltrarDTO { 
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina,
                Titulo = titulo,
                GeneroId = generoId,
                EnCines = enCines,
                ProximosEstrenos = proximosEstrenos,
                CampoOrdenar = campoOrdenar,
                OrdenAscendente = ordenAscendente
            };
          
            return ValueTask.FromResult(peliculasFiltrarDTO);
        }
    }
}
