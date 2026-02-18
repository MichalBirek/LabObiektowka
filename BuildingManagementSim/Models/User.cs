namespace BuildingManagementSim.Models;

public abstract class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; protected set; } = "User";
}
