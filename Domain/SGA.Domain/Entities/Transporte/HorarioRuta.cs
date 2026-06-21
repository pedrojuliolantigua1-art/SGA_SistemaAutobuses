using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Transporte
{
    public class HorarioRuta : BaseEntity
    {
        public int RutaId { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegadaEstimada { get; set; }
        public bool Activo { get; set; }
    }
}
