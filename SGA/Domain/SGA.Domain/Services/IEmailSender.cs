namespace SGA.Domain.Services
{

    public interface IEmailSender
    {
        /// <summary>
        /// Envia un correo a un destinatario.
        /// </summary>
        /// <param name="destinatario">Correo del receptor</param>
        /// <param name="asunto">Asunto del correo</param>
        /// <param name="cuerpo">Contenido HTML o texto del correo</param>
        Task EnviarAsync(string destinatario, string asunto, string cuerpo);
    }
}