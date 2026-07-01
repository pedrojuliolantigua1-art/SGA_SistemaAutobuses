namespace SGA.Domain.Services
{

    public interface IAlmacenamientoArchivos
    {
        /// <summary>
        /// Sube un archivo y retorna la URL publica y el PublicId del servicio.
        /// </summary>
        /// <param name="contenido">Bytes del archivo</param>
        /// <param name="nombreArchivo">Nombre con extension ej: foto.jpg</param>
        /// <param name="carpeta">Carpeta en Cloudinary ej: "autobuses" o "incidencias"</param>
        Task<ResultadoSubida> SubirAsync(byte[] contenido, string nombreArchivo, string carpeta);

        // Este elimina un archivo usando el PublicId que retorno SubirAsync.
        Task EliminarAsync(string publicId);
    }

    public sealed record ResultadoSubida(
        string UrlPublica,   
        string PublicId,     
        string NombreArchivo
    );
}
