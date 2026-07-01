using Microsoft.Data.SqlClient;
using System.Data;
using SGA.Infrastructure.Persistence.Abstractions;

namespace SGA.Infrastructure.Persistence.Common
{
    public abstract class SqlRepositoryBase
    {
        private readonly ISqlConnectionfactory _factory;

        protected SqlRepositoryBase(ISqlConnectionfactory factory)
            => _factory = factory;

        protected async Task<IReadOnlyList<T>> QueryAsync<T>(
            string sp,
            Func<SqlReaderRow, T> map,
            params SqlParameter[] parameters)
        {
            var result = new List<T>();
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, sp, parameters);
            await using var reader = await cmd.ExecuteReaderAsync();

            var row = new SqlReaderRow(reader);
            while (await reader.ReadAsync())
            result.Add(map(row));

            return result.AsReadOnly();
        }

        protected async Task<T?> QuerySingleOrDefaultAsync<T>(
            string sp,
            Func<SqlReaderRow, T> map,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, sp, parameters);
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync()) return default;
            return map(new SqlReaderRow(reader));
        }

        protected async Task ExecuteAsync(
            string sp,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, sp, parameters);
            await cmd.ExecuteNonQueryAsync();
        }

        protected async Task<int> ExecuteScalarAsync(
            string sp,
            params SqlParameter[] parameters)
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, sp, parameters);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // para SPs que devuelven multiples result sets (reportes)
        protected async Task<(IReadOnlyList<T1> First, IReadOnlyList<T2> Second)> QueryMultipleAsync<T1, T2>(
            string sp,
            Func<SqlReaderRow, T1> mapFirst,
            Func<SqlReaderRow, T2> mapSecond,
            params SqlParameter[] parameters)
        {
            var first = new List<T1>();
            var second = new List<T2>();

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();
            await using var cmd = BuildCommand(con, sp, parameters);
            await using var reader = await cmd.ExecuteReaderAsync();

            var row1 = new SqlReaderRow(reader);
            while (await reader.ReadAsync())
                first.Add(mapFirst(row1));

            await reader.NextResultAsync();

            var row2 = new SqlReaderRow(reader);
            while (await reader.ReadAsync())
                second.Add(mapSecond(row2));

            return (first.AsReadOnly(), second.AsReadOnly());
        }

        public static SqlParameter Param(string name, object? value)
            => new(name, value ?? DBNull.Value);

        private static SqlCommand BuildCommand(
            SqlConnection connection,
            string sp,
            IEnumerable<SqlParameter> parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sp;
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var p in parameters)
                cmd.Parameters.Add(p);
            return cmd;
        }
    }
}