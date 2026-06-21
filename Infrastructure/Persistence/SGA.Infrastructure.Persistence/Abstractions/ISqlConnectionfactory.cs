using Microsoft.Data.SqlClient;

namespace SGA.Infrastructure.Persistence.Abstractions
{
    public interface ISqlConnectionfactory
    {
        SqlConnection CreateConnection();
    }
}
