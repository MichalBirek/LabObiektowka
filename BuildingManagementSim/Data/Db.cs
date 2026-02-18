using Microsoft.Data.SqlClient;

namespace BuildingManagementSim.Data;

public static class Db
{
    public static string ConnectionString =
        "Server=localhost;Database=BuildingSimDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    public static SqlConnection GetConnection()
        => new SqlConnection(ConnectionString);
}
