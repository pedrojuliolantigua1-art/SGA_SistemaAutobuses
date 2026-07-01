using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Notificaciones
{
    public class Notificacion : BaseEntity
    {
        public int UsuarioTransporteId { get; set; }

        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;

        public DateTime FechaHora { get; set; }
        public bool Leida { get; set; }
    }
}
