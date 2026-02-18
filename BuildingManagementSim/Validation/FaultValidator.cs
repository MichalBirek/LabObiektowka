using BuildingManagementSim.Models;

namespace BuildingManagementSim.Validation;

public class FaultValidator : IValidator<Fault>
{
    public void Validate(Fault f)
    {
        if (string.IsNullOrWhiteSpace(f.Title) || f.Title.Length < 3)
            throw new ValidationException("Tytuł musi mieć minimum 3 znaki.");
        if (string.IsNullOrWhiteSpace(f.Description) || f.Description.Length < 5)
            throw new ValidationException("Opis musi mieć minimum 5 znaków.");
    }
}
