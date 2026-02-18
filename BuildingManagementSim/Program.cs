using BuildingManagementSim.Data;
using BuildingManagementSim.Models;
using BuildingManagementSim.Services;
using BuildingManagementSim.Validation;

var userRepo = new UserRepository();
var faultRepo = new FaultRepository();
var eventRepo = new EventRepository();

var sensorService = new SensorService();
var deviceService = new DeviceService();
var buildingState = new BuildingStateService();

var faultValidator = new FaultValidator();

Console.WriteLine("=== Building Management Simulator ===");

User? currentUser = null;

while (currentUser == null)
{
    Console.Write("Login: ");
    var login = Console.ReadLine() ?? "";
    Console.Write("Hasło: ");
    var pass = Console.ReadLine() ?? "";

    currentUser = userRepo.Login(login, pass);

    if (currentUser == null)
    {
        Console.WriteLine("Błędne dane logowania!");
        eventRepo.Add(new EventLog { Type = "ALARM", Message = $"Nieudane logowanie: {login}" });
    }
}

Console.WriteLine($"Zalogowano jako: {currentUser.Login} ({currentUser.Role})");
eventRepo.Add(new EventLog { Type = "INFO", Message = $"Logowanie: {currentUser.Login}" });

bool run = true;
while (run)
{
    Console.WriteLine("\n--- MENU ---");
    Console.WriteLine("1. Pokaż temperatury (powietrze/woda)");
    Console.WriteLine("2. Zgłoś usterkę");
    Console.WriteLine("3. Lista usterek");
    Console.WriteLine("4. Zgłoś podejrzane zdarzenie");

    if (currentUser is AdminUser)
    {
        Console.WriteLine("5. Zmień status usterki (ADMIN)");
        Console.WriteLine("6. Sterowanie urządzeniami (ADMIN)");
        Console.WriteLine("7. Zmień stan budynku (ADMIN)");
        Console.WriteLine("8. Historia zdarzeń (ADMIN)");
    }

    Console.WriteLine("0. Wyjście");

    Console.Write("Wybór: ");
    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                Console.WriteLine($"Temp. powietrza: {sensorService.GetAirTemp()} C");
                Console.WriteLine($"Temp. wody: {sensorService.GetWaterTemp()} C");
                break;

            case "2":
                Console.Write("Tytuł: ");
                var title = Console.ReadLine() ?? "";
                Console.Write("Opis: ");
                var desc = Console.ReadLine() ?? "";

                var f = new Fault
                {
                    Title = title,
                    Description = desc,
                    Status = "New",
                    CreatedAt = DateTime.Now,
                    CreatedByUserId = currentUser.Id
                };

                faultValidator.Validate(f);
                faultRepo.Add(f);

                Console.WriteLine("✅ Usterka zapisana.");
                eventRepo.Add(new EventLog { Type = "FAULT", Message = $"Nowa usterka: {title}" });
                break;

            case "3":
                var faults = faultRepo.GetAll();
                foreach (var x in faults)
                    Console.WriteLine($"{x.Id}. {x.Title} | {x.Status} | {x.CreatedAt}");
                break;

            case "4":
                Console.Write("Opis zdarzenia: ");
                var msg = Console.ReadLine() ?? "";
                if (msg.Length < 3) throw new ValidationException("Opis za krótki.");
                eventRepo.Add(new EventLog { Type = "ALARM", Message = msg });
                Console.WriteLine("✅ Zapisano zdarzenie alarmowe.");
                break;

            case "5" when currentUser is AdminUser:
                Console.Write("ID usterki: ");
                if (!int.TryParse(Console.ReadLine(), out var fid))
                    throw new ValidationException("ID musi być liczbą.");

                var fault = faultRepo.GetById(fid);
                if (fault == null) throw new Exception("Nie znaleziono usterki.");

                Console.Write("Nowy status (New/InProgress/Done): ");
                var st = Console.ReadLine() ?? "";
                if (st != "New" && st != "InProgress" && st != "Done")
                    throw new ValidationException("Status musi być: New, InProgress lub Done.");

                fault.Status = st;
                faultRepo.Update(fault);

                Console.WriteLine("✅ Status zaktualizowany.");
                eventRepo.Add(new EventLog { Type = "INFO", Message = $"Zmiana statusu usterki ID={fid} na {st}" });
                break;

            case "6" when currentUser is AdminUser:
                var devs = deviceService.GetDevices();
                foreach (var d in devs)
                    Console.WriteLine($"{d.Id}. {d.Name} ({d.DeviceType}) | {(d.IsOn ? "ON" : "OFF")}");

                Console.Write("Podaj ID do przełączenia: ");
                if (!int.TryParse(Console.ReadLine(), out var did))
                    throw new ValidationException("ID musi być liczbą.");

                deviceService.Toggle(did);
                Console.WriteLine("✅ Przełączono urządzenie.");
                eventRepo.Add(new EventLog { Type = "INFO", Message = $"Przełączono urządzenie ID={did}" });
                break;

            case "7" when currentUser is AdminUser:
                Console.Write("Stan budynku (Open/Closed/Alarm): ");
                var ns = Console.ReadLine() ?? "";
                if (ns != "Open" && ns != "Closed" && ns != "Alarm")
                    throw new ValidationException("Stan musi być: Open, Closed lub Alarm.");

                buildingState.SetState(ns);
                Console.WriteLine($"✅ Nowy stan budynku: {buildingState.State}");
                eventRepo.Add(new EventLog { Type = "INFO", Message = $"Zmiana stanu budynku: {ns}" });
                break;

            case "8" when currentUser is AdminUser:
                var events = eventRepo.GetAll();
                foreach (var e in events)
                    Console.WriteLine($"{e.CreatedAt} | {e.Type} | {e.Message}");
                break;

            case "0":
                run = false;
                break;

            default:
                Console.WriteLine("Nieznana opcja.");
                break;
        }
    }
    catch (ValidationException ex)
    {
        Console.WriteLine($"❌ Walidacja: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Błąd: {ex.Message}");
    }
}
