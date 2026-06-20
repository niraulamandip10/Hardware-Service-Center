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
        await connection.ExecuteAsync(CreateTicketTable);
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

    public const string CreateTicketTable = @"CREATE TABLE IF NOT EXISTS tickets
(
    id               int generated always as identity primary key ,
    ticketno          VARCHAR(50) NOT NULL,
    customerid        INTEGER     NOT NULL,
    deviceid          INTEGER     NOT NULL,
    technicianid      INTEGER,
    ticketdescription TEXT,
    recdate           TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP,
    recbyid          INTEGER     NOT NULL,
    ticketstatus      INTEGER     NOT NULL DEFAULT 1,

    CONSTRAINT fk_ticket_customer
        FOREIGN KEY (customerid)
            REFERENCES customer (id),

    CONSTRAINT fk_ticket_device
        FOREIGN KEY (deviceid)
            REFERENCES device (id),

    CONSTRAINT fk_ticket_technician
        FOREIGN KEY (technicianid)
            REFERENCES technician (id),
   CONSTRAINT fk_ticket_RecById
        FOREIGN KEY (recbyid)
            REFERENCES users (id),


    CONSTRAINT chk_ticket_status
        CHECK (ticketstatus IN (1, 2, 3, 4, 5)))";

}


