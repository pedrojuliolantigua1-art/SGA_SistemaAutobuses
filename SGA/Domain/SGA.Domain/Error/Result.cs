namespace SGA.Domain.Error
{
    public class Result
    {
        public bool EsExitoso { get; }
        public bool EsFallo => !EsExitoso;
        public Error? Error { get; }

        protected Result(bool esExitoso, Error? error)
        {
            if (esExitoso && error is not null && !string.IsNullOrWhiteSpace(error.Codigo))
            {
                throw new InvalidOperationException("Si tuvo exito no debe hacer error");
            }

            if (!esExitoso && error is null)
            {
                throw new InvalidOperationException("Debe haber un error");
            }

            EsExitoso = esExitoso;
            Error = error;
        }

        public static Result Ok() => new(true, null);
        public static Result Fallo(Error error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Valor { get; }

        private Result(T? valor, bool esExitoso, Error? error)
            : base(esExitoso, error)
        {
            Valor = valor;
        }

        public static Result<T> Ok(T valor) => new(valor, true, null);
        public static new Result<T> Fallo(Error error) => new(default, false, error);
    }
}
