namespace BuildingManagementSim.Services;

public class BuildingStateService
{
    public string State { get; private set; } = "Open";

    public void SetState(string newState)
    {
        State = newState;
    }
}
