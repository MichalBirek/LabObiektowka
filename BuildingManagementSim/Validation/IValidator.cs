namespace BuildingManagementSim.Validation;

public interface IValidator<T>
{
    void Validate(T item);
}
