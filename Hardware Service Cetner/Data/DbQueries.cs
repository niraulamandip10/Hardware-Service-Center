using Dapper;


namespace Hardware_Service_Cetner.Data;

public class DbQueries
{
    private readonly DapperContext _dapperContext;

    public DbQueries(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task IntializeAsync()
    {
        using var connection = _dapperContext.CreateConnection();
        await connection.ExecuteAsync(CreateCustomerTable);
    }

    public const string CreateCustomerTable = @"CREATE TABLE IF NOT EXISTS customer (
    id int generated always as identity primary key ,
    name varchar(100),
    email varchar(100),
    phone varchar(50),
    address varchar(50),
    Status integer not null default 1)";
}