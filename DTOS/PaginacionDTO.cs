using ApiPeliculas.Utilidades;
using Microsoft.IdentityModel.Tokens;

namespace ApiPeliculas.DTOS
{
    public class PaginacionDTO
    {
        private const int PaginaValrInicial = 1;
        private const int RecordspaginaInici = 10;
        public int Pagina { get; set; } = PaginaValrInicial;
        private int recordsPorPagina = RecordspaginaInici;
        private readonly int catindadMaximaPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > catindadMaximaPorPagina) ? catindadMaximaPorPagina : value;
            }
        }

        public static ValueTask<PaginacionDTO> BindAsync(HttpContext httpContext) 
        {
            /*
            var pagina = httpContext.Request.Query[nameof(Pagina)];
            var recordspagina = httpContext.Request.Query[nameof(recordsPorPagina)];

            var paginaInt = pagina.IsNullOrEmpty() ? PaginaValrInicial : int.Parse(pagina.ToString());
            var recordsInt = recordspagina.IsNullOrEmpty()? RecordspaginaInici : int.Parse(recordspagina.ToString());

            */

            var pagina = httpContext.ExatrerValorODefecto(nameof(Pagina),PaginaValrInicial);
            var recordspagina = httpContext.ExatrerValorODefecto(nameof(recordsPorPagina),RecordspaginaInici);

            var resultado = new PaginacionDTO
            {
                Pagina = pagina,
                RecordsPorPagina = recordspagina
            };

            return ValueTask.FromResult(resultado);
        }
    }
}
