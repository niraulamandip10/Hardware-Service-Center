using System.Data;
using Npgsql;

namespace Hardware_Service_Cetner.Data;

public class IDbConnectionProvider
{
    private readonly IConfiguration _configuration;
    public IDbConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));
    }
}