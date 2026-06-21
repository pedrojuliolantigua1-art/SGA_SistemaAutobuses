using Microsoft.Data.SqlClient;
using SGA.Infrastructure.Persistence.Abstractions;

namespace SGA.Infrastructure.Persistence.Conexion
{
    public sealed class SqlConnectionfactory : ISqlConnectionfactory
    {
        private readonly string _connectionString;

        public SqlConnectionfactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "La conexion no puede estar vacia");
                _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
