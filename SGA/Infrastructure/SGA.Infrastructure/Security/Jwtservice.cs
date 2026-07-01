using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGA.Domain.Entities.Usuarios;
using SGA.Domain.Enum;
using SGA.Domain.Repository.Interfaces;
using SGA.Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGA.Infrastructure.Security
{

    public sealed class JwtService : IJwt
    {
        private readonly JwtOptions _options;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerarToken(UsuarioTransporte usuario)
        {
            var clave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey));

            var credenciales = new SigningCredentials(
                clave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo ?? string.Empty),
                new Claim(ClaimTypes.Name,               $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Role,               usuario.RolSistema.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidarToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var clave = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.SecretKey));

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = clave,
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int ObtenerUsuarioId(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var sub = jwt.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(sub, out var id)
                ? id
                : throw new InvalidOperationException(
                    "El token no contiene un Id de usuario valido.");
        }

        public RolUsuario ObtenerRol(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var rol = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role)?.Value;

            return Enum.TryParse<RolUsuario>(rol, out var rolUsuario)
                ? rolUsuario
                : throw new InvalidOperationException(
                    "El token no contiene un Rol valido.");
        }
    }
}