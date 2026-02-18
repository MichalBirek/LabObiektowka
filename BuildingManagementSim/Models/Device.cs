namespace BuildingManagementSim.Models;

public abstract class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsOn { get; set; }
    public abstract string DeviceType { get; }
}
