using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Utilidades
{

    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            
            if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
           // double cantidadPaginas = Math.Ceiling(cantidad / recordsPorPagina);
            httpContext.Response.Headers.Append("cantidaRegistros", cantidad.ToString());
        }
    }
}
