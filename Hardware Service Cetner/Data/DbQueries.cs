using Dapper;


namespace Hardware_Service_Cetner.Data;

public class DbQueries
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public DbQueries(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public async Task IntializeAsync()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        await connection.ExecuteAsync(CreateCustomerTable);
        await connection.ExecuteAsync(CreateTechnicianTable);
        await connection.ExecuteAsync(CreateUserTable);
        await connection.ExecuteAsync(CreateDeviceTable);
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

    public const string CreateUserTable = @"
CREATE TABLE IF NOT EXISTS users (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    name VARCHAR(100) NOT NULL,

    email VARCHAR(100),

    phone VARCHAR(30),

    address VARCHAR(200),

    username VARCHAR(50) NOT NULL UNIQUE,

    password VARCHAR(200) NOT NULL,

    registrationdate TIMESTAMP NOT NULL,

    isactive BOOLEAN NOT NULL DEFAULT TRUE
);";

    public const string CreateDeviceTable = @"CREATE TABLE IF NOT EXISTS device (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description varchar(500),
    Status bool not null default true );";
    
    

}


