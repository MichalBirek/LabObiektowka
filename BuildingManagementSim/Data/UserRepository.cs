using BuildingManagementSim.Models;
using Microsoft.Data.SqlClient;

namespace BuildingManagementSim.Data;

public class UserRepository
{
    public User? Login(string login, string password)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand("SELECT * FROM Users WHERE Login=@l AND PasswordHash=@p", con);
        cmd.Parameters.AddWithValue("@l", login);
        cmd.Parameters.AddWithValue("@p", password);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        var role = (string)r["Role"];
        User u = role switch
        {
            "Admin" => new AdminUser(),
            "Resident" => new ResidentUser(),
            "Employee" => new EmployeeUser(),
            _ => new ResidentUser()
        };

        u.Id = (int)r["Id"];
        u.Login = (string)r["Login"];
        u.Password = password;

        return u;
    }
}
