using BuildingManagementSim.Models;

namespace BuildingManagementSim.Services;

public class DeviceService
{
    private readonly List<Device> _devices = new()
    {
        new LightDevice { Id = 1, Name = "Main lights", IsOn = false },
        new CircuitDevice { Id = 2, Name = "Circuit A", IsOn = true }
    };

    public List<Device> GetDevices() => _devices;

    public void Toggle(int id)
    {
        var d = _devices.FirstOrDefault(x => x.Id == id);
        if (d == null) throw new Exception("Nie znaleziono urządzenia.");
        d.IsOn = !d.IsOn;
    }
}
