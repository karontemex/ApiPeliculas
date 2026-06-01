using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Validaciones
{
    public class CrearActorDtoValidador: AbstractValidator<CrearActorDto>
    {
        public CrearActorDtoValidador()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.LongitudMaximaMensaje);

            var FechaMinima = new DateTime(1900,1,1);
            RuleFor(x => x.FechaNacimiento).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .GreaterThan(FechaMinima).WithMessage("La fecha de nacimiento del actor debe ser mayor a {ComparisonValue}");
        }

    }
}
