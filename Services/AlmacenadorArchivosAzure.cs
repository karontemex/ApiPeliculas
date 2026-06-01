using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ApiPeliculas.Services
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private string? connectionStrng;
        AlmacenadorArchivosAzure(IConfiguration configuration) 
        {
                connectionStrng = configuration.GetConnectionString("AzureStorage");
        }
        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionStrng, contenedor);
            await cliente.CreateIfNotExistsAsync(); //Crea el contenedor si no existe, si existe no hace nada
            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var extension = Path.GetExtension(archivo.FileName);
            var NombreArchivo = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(NombreArchivo);
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeader);
            return blob.Uri.ToString();
        }

        public async Task Borrar(string? ruta, string contenedor)
        {
            if (ruta != null)
            {
                var cliente = new BlobContainerClient(connectionStrng, contenedor);
                await cliente.CreateIfNotExistsAsync();
                var nombreArchivo = Path.GetFileName(ruta);
                var blob = cliente.GetBlobClient(nombreArchivo);
                await blob.DeleteIfExistsAsync();

            }

            return;
        }
    }
}
