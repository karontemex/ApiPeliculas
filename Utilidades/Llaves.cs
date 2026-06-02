using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;

namespace ApiPeliculas.Utilidades
{
    public static class Llaves
    {
        public const string IssuerPropio = "ApiPeliculas";
        public const string SeccionLlaves = "Authentication:Schemes:Bearer:SigningKeys";
        public const string SeccionLlaves_Emisor = "Issuer";
        public const String SeccionLlaves_Valor = "W/Etp5F7UhOUn7cOyEAs3YYLRM99a53eI9HleodGH4E=";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration) => ObtenerLlave(configuration, IssuerPropio);
        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer) { 
            var signinKey = configuration.GetSection(SeccionLlaves)
                .GetChildren()
                .SingleOrDefault(llave => llave[SeccionLlaves_Emisor] == issuer);

            if (signinKey is not null && signinKey[SeccionLlaves_Valor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }

        public static IEnumerable<SecurityKey> ObtenerTodasLlaves(IConfiguration configuration, string issuer)
        {
            var signinKeys = configuration.GetSection(SeccionLlaves)
                .GetChildren();                

            foreach(var key in signinKeys)
            {
                if (key[SeccionLlaves_Valor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
            }
        }

    }
}
