using ApiPeliculas.DTOS;
using FluentValidation;

namespace ApiPeliculas.Validaciones
{
    public class EditarClaimDTOValidador:AbstractValidator<EditarClaimDTO>
    {
        public EditarClaimDTOValidador() {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                 .MaximumLength(250).WithMessage(Utilidades.LongitudMaximaMensaje)
                 .EmailAddress().WithMessage(Utilidades.EmailMessage);
        }
    }
}
