namespace ApiPeliculas.Services
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccesssor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccesor)
        {
            this.env = env;
            this.httpContextAccesssor = httpContextAccesor;
        }
        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string ruta = Path.Combine(folder, nombreArchivo);
            using(var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                var contenido = ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }

            var url = $"{httpContextAccesssor.HttpContext.Request.Scheme}://{httpContextAccesssor.HttpContext.Request.Host}/{contenedor}/{nombreArchivo}";
            return url;
        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (ruta != null)
            {
                var archivo = Path.GetFileName(ruta);
                var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, archivo);
                if (File.Exists(directorioArchivo))
                {
                    File.Delete(directorioArchivo);
                }
            }
            return Task.CompletedTask;
        }
    }
}
