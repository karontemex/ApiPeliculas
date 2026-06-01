using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Validaciones
{
    public class CrearComentarioDtoValidador: AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDtoValidador()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(500).WithMessage(Utilidades.LongitudMaximaMensaje);
        }
    }
}
