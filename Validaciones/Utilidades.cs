namespace ApiPeliculas.Validaciones
{
    public static class 
        Utilidades
    {
        public static string CampoRequeridoMensaje = "El Campo {PropertyName} es Requerido";
        public static string LongitudMaximaMensaje = "El Campo {PropertyName} no debe exceder los {MaxLength} caracteres";
        public static string PrimeraMayusculaMensaje = "El Campo {PropertyName} debe iniciar con mayuscula";
        public static string EmailMessage = "El campo {PropertyName} debe ser una email valido";
    

    public static bool PrimeraMayusculas(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primerLetra = valor[0].ToString();
            return primerLetra == primerLetra.ToUpper();

        }
    }
}
