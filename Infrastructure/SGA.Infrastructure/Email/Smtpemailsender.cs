using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SGA.Domain.Services;

namespace SGA.Infrastructure.Email
{
 
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;

        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task EnviarAsync(
            string destinatario,
            string asunto,
            string cuerpo)
        {
            using var cliente = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(
                    _options.UserName,
                    _options.Password)
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress(_options.FromAddress, _options.FromName),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            await cliente.SendMailAsync(mensaje);
        }
    }
}