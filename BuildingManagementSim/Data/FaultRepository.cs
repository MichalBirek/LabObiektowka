using BuildingManagementSim.Models;
using Microsoft.Data.SqlClient;

namespace BuildingManagementSim.Data;

public class FaultRepository : IRepository<Fault>
{
    public void Add(Fault f)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand(
            "INSERT INTO Faults(Title, Description, Status, CreatedAt, CreatedByUserId, RoomId) VALUES (@t,@d,@s,@c,@u,@r)",
            con);

        cmd.Parameters.AddWithValue("@t", f.Title);
        cmd.Parameters.AddWithValue("@d", f.Description);
        cmd.Parameters.AddWithValue("@s", f.Status);
        cmd.Parameters.AddWithValue("@c", f.CreatedAt);
        cmd.Parameters.AddWithValue("@u", f.CreatedByUserId);
        cmd.Parameters.AddWithValue("@r", (object?)f.RoomId ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public List<Fault> GetAll()
    {
        var list = new List<Fault>();
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand("SELECT * FROM Faults", con);
        using var r = cmd.ExecuteReader();

        while (r.Read())
        {
            list.Add(new Fault
            {
                Id = (int)r["Id"],
                Title = (string)r["Title"],
                Description = (string)r["Description"],
                Status = (string)r["Status"],
                CreatedAt = (DateTime)r["CreatedAt"],
                CreatedByUserId = (int)r["CreatedByUserId"],
                RoomId = r["RoomId"] == DBNull.Value ? null : (int?)r["RoomId"]
            });
        }

        return list;
    }

    public Fault? GetById(int id)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand("SELECT * FROM Faults WHERE Id=@id", con);
        cmd.Parameters.AddWithValue("@id", id);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Fault
        {
            Id = (int)r["Id"],
            Title = (string)r["Title"],
            Description = (string)r["Description"],
            Status = (string)r["Status"],
            CreatedAt = (DateTime)r["CreatedAt"],
            CreatedByUserId = (int)r["CreatedByUserId"],
            RoomId = r["RoomId"] == DBNull.Value ? null : (int?)r["RoomId"]
        };
    }

    public void Update(Fault f)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand(
            "UPDATE Faults SET Title=@t, Description=@d, Status=@s, RoomId=@r WHERE Id=@id", con);

        cmd.Parameters.AddWithValue("@t", f.Title);
        cmd.Parameters.AddWithValue("@d", f.Description);
        cmd.Parameters.AddWithValue("@s", f.Status);
        cmd.Parameters.AddWithValue("@r", (object?)f.RoomId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", f.Id);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand("DELETE FROM Faults WHERE Id=@id", con);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
