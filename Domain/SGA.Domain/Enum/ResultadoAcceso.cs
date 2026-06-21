using System;
using System.Collections.Generic;
using System.Text;

namespace SGA.Domain.Enum
{
    public enum ResultadoAcceso
    {
        Permitido = 1,
        Denegado = 2,
        AutorizacionVencida = 3,
        SinCupo = 4,
        UsuarioInactivo = 5,
        SaldoInsuficiente = 6,
        SinAutorizacion = 7,
        ViajeNoDisponible = 8,
        AutorizacionInvalida = 9
    }
}
