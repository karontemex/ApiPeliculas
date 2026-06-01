using ApiPeliculas.DTOS;
using ApiPeliculas.Migrations;
using ApiPeliculas.Repository;
using FluentValidation;
using static System.Net.WebRequestMethods;

namespace ApiPeliculas.Validaciones
{
    public class CrearGeneroDtoValidador: AbstractValidator<CrearGeneroDTO>
    {
        
        public CrearGeneroDtoValidador(IRepositoryGeneros repositoryGeneros,IHttpContextAccessor httpContextAccessor) {
            
            var valorRutaId = httpContextAccessor.HttpContext.Request.RouteValues["id"];
            var id = 0;

            if (valorRutaId is string valorString) { 
                int.TryParse(valorString, out id );
            } 

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(50).WithMessage(Utilidades.LongitudMaximaMensaje)
                .Must(Utilidades.PrimeraMayusculas).WithMessage(Utilidades.PrimeraMayusculaMensaje)
                .MustAsync(async(Nombre,_) => {
                    var existe = await repositoryGeneros.GeneroExists(id,Nombre);
                    return existe;
                }).WithMessage(g => "Ya existe un genero con el nombre: {g.Nombre}");
        }

        
    }
}
