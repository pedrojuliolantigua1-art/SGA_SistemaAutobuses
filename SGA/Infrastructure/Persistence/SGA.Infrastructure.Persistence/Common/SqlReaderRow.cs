using Microsoft.Data.SqlClient;

namespace SGA.Infrastructure.Persistence.Common
{
    public sealed class SqlReaderRow
    {
        private readonly SqlDataReader _r;
        private readonly Dictionary<string, int> _cache = new(16, StringComparer.Ordinal);

        public SqlReaderRow(SqlDataReader reader) => _r = reader;

        private int Ord(string col)
        {
            if (!_cache.TryGetValue(col, out var i))
                _cache[col] = i = _r.GetOrdinal(col);
            return i;
        }

        public int Int(string col) => _r.GetInt32(Ord(col));

        public int? NullableInt(string col)
        {
            var i = Ord(col);
            return _r.IsDBNull(i) ? null : _r.GetInt32(i);
        }

        public string? Str(string col)
        {
            var i = Ord(col);
            return _r.IsDBNull(i) ? null : _r.GetString(i);
        }

        public decimal Dec(string col) => _r.GetDecimal(Ord(col));

        public bool Bool(string col) => _r.GetBoolean(Ord(col));

        public DateTime DateTime(string col) => _r.GetDateTime(Ord(col));

        public DateTime? NullableDateTime(string col)
        {
            var i = Ord(col);
            return _r.IsDBNull(i) ? null : _r.GetDateTime(i);
        }

        public TimeSpan TimeSpan(string col) => (TimeSpan)_r.GetValue(Ord(col));

        public TEnum Enum<TEnum>(string col) where TEnum : struct, Enum
            => (TEnum)System.Enum.ToObject(typeof(TEnum), _r.GetInt32(Ord(col)));
    }
}