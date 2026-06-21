using Microsoft.Data.SqlClient;
using SGA.Infrastructure.Persistence.Abstractions;

namespace SGA.Infrastructure.Persistence.Common
{
    /// Clase base para todos los repositorios

    public abstract class SqlRepositoryBase
    {
        private readonly ISqlConnectionfactory _factory;

        protected SqlRepositoryBase(ISqlConnectionfactory factory)
        {
            _factory = factory;
        }

        //Estos son de lectura
        //Ejecuta el SP que retorna multiples filas
        protected async Task<IReadOnlyList<T>> QueryAsync<T>(
            string storedProcedure,
            Func<SqlDataReader, T> map,
            params SqlParameter[] parameters)
        {
            var result = new List<T>();

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, storedProcedure, parameters);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.Add(map(reader));

            return result.AsReadOnly();
        }

        //ejcuta un SP que retorna una fila o null
        protected async Task<T?> QuerySingleOrDefaultAsync<T>(
            string storedProcedure,
            Func<SqlDataReader, T> map,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, storedProcedure, parameters);
            await using var reader = await cmd.ExecuteReaderAsync();

            return await reader.ReadAsync() ? map(reader) : default;
        }

        //esto son de escritura
        //Ejecuta un SP que no retorna filas o sea Update y Delete
        protected async Task ExecuteAsync(
            string storedProcedure,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, storedProcedure, parameters);
            await cmd.ExecuteNonQueryAsync();
        }


        // Ejecuta el procedimiento almacenado de insert y retorna el Id generado. Tambien utilizo Output insert para obtener el id creado
        protected async Task<int> ExecuteScalarAsync(
            string storedProcedure,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, storedProcedure, parameters);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // helpers de mapeo para mapear los datos 

        public static SqlParameter Param(string name, object? value)
            => new(name, value ?? DBNull.Value);

        public static string? GetString(SqlDataReader r, string col)
        {
            var i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }

        public static int GetInt(SqlDataReader r, string col)
            => r.GetInt32(r.GetOrdinal(col));

        public static int? GetNullableInt(SqlDataReader r, string col)
        {
            var i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetInt32(i);
        }

        public static decimal GetDecimal(SqlDataReader r, string col)
            => r.GetDecimal(r.GetOrdinal(col));

        public static bool GetBool(SqlDataReader r, string col)
            => r.GetBoolean(r.GetOrdinal(col));

        public static DateTime GetDateTime(SqlDataReader r, string col)
            => r.GetDateTime(r.GetOrdinal(col));

        public static DateTime? GetNullableDateTime(SqlDataReader r, string col)
        {
            var i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetDateTime(i);
        }

        public static TimeSpan GetTimeSpan(SqlDataReader r, string col)
            => (TimeSpan)r.GetValue(r.GetOrdinal(col));

        public static TEnum GetEnum<TEnum>(SqlDataReader r, string col)
            where TEnum : struct, Enum
            => (TEnum)Enum.ToObject(typeof(TEnum), GetInt(r, col));

        private static SqlCommand BuildCommand(
            SqlConnection connection,
            string storedProcedure,
            IEnumerable<SqlParameter> parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = storedProcedure;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (var p in parameters)
                cmd.Parameters.Add(p);

            return cmd;
        }
    }
}
