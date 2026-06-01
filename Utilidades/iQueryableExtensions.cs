using ApiPeliculas.DTOS;

namespace ApiPeliculas.Utilidades
{
    public static class iQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO  paginacionDTO)
        {
            return queryable.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina).Take(paginacionDTO.RecordsPorPagina);
        }
    }
}
