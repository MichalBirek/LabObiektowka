using BuildingManagementSim.Models;
using Microsoft.Data.SqlClient;

namespace BuildingManagementSim.Data;

public class EventRepository : IRepository<EventLog>
{
    public void Add(EventLog e)
    {
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand(
            "INSERT INTO Events(Type, Message, CreatedAt) VALUES (@t,@m,@c)", con);

        cmd.Parameters.AddWithValue("@t", e.Type);
        cmd.Parameters.AddWithValue("@m", e.Message);
        cmd.Parameters.AddWithValue("@c", e.CreatedAt);

        cmd.ExecuteNonQuery();
    }

    public List<EventLog> GetAll()
    {
        var list = new List<EventLog>();
        using var con = Db.GetConnection();
        con.Open();

        var cmd = new SqlCommand("SELECT * FROM Events ORDER BY CreatedAt DESC", con);
        using var r = cmd.ExecuteReader();

        while (r.Read())
        {
            list.Add(new EventLog
            {
                Id = (int)r["Id"],
                Type = (string)r["Type"],
                Message = (string)r["Message"],
                CreatedAt = (DateTime)r["CreatedAt"]
            });
        }

        return list;
    }

    public EventLog? GetById(int id) => null;
    public void Update(EventLog item) { }
    public void Delete(int id) { }
}
