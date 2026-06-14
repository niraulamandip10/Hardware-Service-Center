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
        await connection.ExecuteAsync(CreateTechnicianTable);
    }

    public const string CreateCustomerTable = @"CREATE TABLE IF NOT EXISTS customer (
    id int generated always as identity primary key ,
    name varchar(100),
    email varchar(100),
    phone varchar(50),
    address varchar(50),
    status integer not null default 1)";

    public const string CreateTechnicianTable =
        @"CREATE TABLE IF NOT EXISTS technician ( 
    id    int generated always as identity primary key,
    name        VARCHAR(255) NOT NULL,
    description varchar(500),
    code        INTEGER       NOT NULL,
    recdate     TIMESTAMP    NOT NULL,
    isactive    BOOLEAN      NOT NULL DEFAULT TRUE)";
}

