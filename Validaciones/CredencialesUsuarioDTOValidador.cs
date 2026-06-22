using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Validaciones
{
    public class CredencialesUsuarioDTOValidador : AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidador() {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilidades.LongitudMaximaMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailMessage);

            RuleFor(x => x.Password).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(30).WithMessage(Utilidades.LongitudMaximaMensaje);


        }
    }
}
