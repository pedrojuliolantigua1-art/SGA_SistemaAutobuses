namespace SGA.Domain.Error
{
    public sealed record Error(string Codigo, string Mensaje)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Codigo)
                ? Mensaje
                : $"{Codigo}: {Mensaje}";
        }
    }
}
