using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGA.Domain.Models.Usuarios
{
    public class EmpleadoDocenteModel : EmpleadoModel
    {
        public string? Especialidad { get; set; } = string.Empty;
        public string? TipoContrato { get; set; } = string.Empty;
    }
}
