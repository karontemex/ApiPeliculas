using ApiPeliculas.Repository;
using AutoMapper;

namespace ApiPeliculas.Filtros
{
    public class FiltroPrueba: IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            //Ejecutar código antes del endpoint handler
            var param1 = (int)context.Arguments[1]!;
            var param2 = (IRepositorioComentarios)context.Arguments[0]!;
            var param3 = (IMapper)context.Arguments[2]!;

            //Mwjor ais podemos filtar por tipo
            var P1 = context.Arguments.OfType<int>().FirstOrDefault();
            var P2 = context.Arguments.OfType<IRepositorioComentarios>().FirstOrDefault();
            var P3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            var resultado = await next(context);
            //Ejecutar código después del endpoint handler
            return resultado;
        }
    }
}
