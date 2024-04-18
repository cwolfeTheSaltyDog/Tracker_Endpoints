using System.Data;

namespace Infrastructure.Database;

internal interface IDbConnectionFactory
{
    IDbConnection OpenConnection();
}
