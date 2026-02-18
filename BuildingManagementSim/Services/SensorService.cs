namespace BuildingManagementSim.Services;

public class SensorService
{
    private readonly Random _rnd = new();

    public double GetAirTemp() => Math.Round(18 + _rnd.NextDouble() * 8, 1);
    public double GetWaterTemp() => Math.Round(40 + _rnd.NextDouble() * 10, 1);
}
