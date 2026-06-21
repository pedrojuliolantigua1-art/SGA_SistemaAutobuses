using SGA.Domain.Enum;

namespace SGA.Domain.Rules
{
    public sealed record EvaluacionAcceso(ResultadoAcceso Resultado, string? MotivoRechazo)
    {
        public bool Permitido => Resultado == ResultadoAcceso.Permitido;

        public static EvaluacionAcceso Aprobada() =>
            new(ResultadoAcceso.Permitido, null);

        public static EvaluacionAcceso Rechazada(ResultadoAcceso resultado, string motivo) =>
            new(resultado, motivo);
    }
}
