using SGA.Domain.Entities.Usuarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Transporte
{
    public class Conductor: UsuarioTransporte
    {
        public string? NumeroLicencia { get; set; }
        public bool Disponible { get; set; }
    }
}
