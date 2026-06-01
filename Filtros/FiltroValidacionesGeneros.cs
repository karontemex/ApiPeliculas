using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Filtros
{
    public class FiltroValidacionesGeneros : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validador = context.HttpContext.RequestServices.GetService<IValidator<CrearGeneroDTO>>();

            if(validador is null)
            {
                return next(context);
            }

            var insumAValidar = context.Arguments.OfType<CrearGeneroDTO>().FirstOrDefault();
            if(insumAValidar is null)
            {
                return TypedResults.Problem("No se encontro el recurso a validar");
            }
            var resultado = await validador.ValidateAsync(insumAValidar);

            if(!resultado.IsValid)
            {
                return TypedResults.ValidationProblem(resultado.ToDictionary());
            }

            return await next(context);
        }
    }
}
