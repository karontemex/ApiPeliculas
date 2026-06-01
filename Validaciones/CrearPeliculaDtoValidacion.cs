using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Validaciones
{
    public class CrearPeliculaDtoValidacion:AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDtoValidacion()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(250).WithMessage(Utilidades.LongitudMaximaMensaje);
            var FechaMinima = new DateTime(1900, 1, 1);
            RuleFor(x => x.FechaEstreno).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .GreaterThan(FechaMinima).WithMessage("La fecha de estreno de la película debe ser mayor a {ComparisonValue}");
        }
    }
}
