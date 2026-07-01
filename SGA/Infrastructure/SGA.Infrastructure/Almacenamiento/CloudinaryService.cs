using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SGA.Domain.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Almacenamiento
{

    public sealed class CloudinaryService : IAlmacenamientoArchivos
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinaryOptions> options)
        {
            var opt = options.Value;

            var account = new Account(
                opt.CloudName,
                opt.ApiKey,
                opt.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<ResultadoSubida> SubirAsync(
            byte[] contenido,
            string nombreArchivo,
            string carpeta)
        {
            using var stream = new MemoryStream(contenido);

            var parametros = new ImageUploadParams
            {
                File = new FileDescription(nombreArchivo, stream),
                Folder = carpeta,
                PublicId = $"{carpeta}/{Path.GetFileNameWithoutExtension(nombreArchivo)}_{DateTime.UtcNow.Ticks}",
                Overwrite = false
            };

            var resultado = await _cloudinary.UploadAsync(parametros);

            if (resultado.Error is not null)
                throw new InvalidOperationException(
                    $"Error al subir archivo a Cloudinary: {resultado.Error.Message}");

            return new ResultadoSubida(
                UrlPublica: resultado.SecureUrl.ToString(),
                PublicId: resultado.PublicId,
                NombreArchivo: nombreArchivo);
        }

        public async Task EliminarAsync(string publicId)
        {
            var parametros = new DeletionParams(publicId);
            var resultado = await _cloudinary.DestroyAsync(parametros);

            if (resultado.Error is not null)
                throw new InvalidOperationException(
                    $"Error al eliminar archivo de Cloudinary: {resultado.Error.Message}");
        }
    }
}