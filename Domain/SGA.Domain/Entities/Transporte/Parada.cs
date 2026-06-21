using SGA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Entities.Transporte
{
    public class Parada : BaseEntity
    {
        public int RutaId { get; set; }
        public string? Nombre { get; set; }
        public string? Referencia {  get; set; }
        public int Orden {  get; set; }
    }
}
