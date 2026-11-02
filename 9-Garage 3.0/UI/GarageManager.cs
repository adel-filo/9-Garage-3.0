using System.Globalization;

public class GarageManager
{
    private readonly IUI _ui;
    private readonly IHandler _handler;

    public GarageManager(IUI ui, IHandler handler)
    {
        _ui = ui;
        _handler = handler;
    }

    public void Run()
    {
        InitializeGarage();

        bool running = true;
        while (running)
        {
            ShowMainMenu();
            string choice = _ui.ReadLine().Trim();

            switch (choice)
            {
                case "1":
                    ListAllParkedVehicles();
                    break;
                case "2":
                    ListVehicleTypesAndCounts();
                    break;
                case "3":
                    ParkVehicleMenu();
                    break;
                case "4":
                    UnparkVehicleMenu();
                    break;
                case "5":
                    FindVehicleByRegistrationNumberMenu();
                    break;
                case "6":
                    SearchVehiclesByPropertiesMenu();
                    break;
                case "7":
                    InitializeGarage();
                    break;
                case "0":
                    _ui.WriteLine("Closing Garage 3.0. Goodbye!");
                    running = false;
                    break;
                default:
                    _ui.WriteLine("Invalid menu choice. Please select a valid option (0-7).");
                    break;
            }
        }
    }

    private void InitializeGarage()
    {
        _ui.WriteLine("\n=== CREATE NEW GARAGE ===");
        int capacity = PromptPositiveInt("Enter garage capacity (number of parking spaces): ");
        _handler.CreateGarage(capacity);
        _ui.WriteLine($"Garage created with a capacity of {capacity} vehicles.");

        string populateChoice = PromptNonEmptyString("Would you like to populate the garage with default vehicles? (y/n): ").ToLowerInvariant();
        if (populateChoice == "y" || populateChoice == "yes")
        {
            int seededCount = _handler.SeedDefaultVehicles();
            _ui.WriteLine($"{seededCount} default vehicle(s) were parked in the garage.");
        }
    }

    private void ShowMainMenu()
    {
        _ui.WriteLine($"\n=== GARAGE 3.0 MAIN MENU (Occupied: {_handler.Count}/{_handler.Capacity}) ===");
        _ui.WriteLine("1. List all parked vehicles");
        _ui.WriteLine("2. List vehicle types and counts");
        _ui.WriteLine("3. Park a vehicle");
        _ui.WriteLine("4. Unpark a vehicle");
        _ui.WriteLine("5. Find vehicle by registration number");
        _ui.WriteLine("6. Search vehicles by properties");
        _ui.WriteLine("7. Create a new garage");
        _ui.WriteLine("0. Exit");
        _ui.Write("Select an option: ");
    }

    private void ListAllParkedVehicles()
    {
        _ui.WriteLine("\n--- Parked Vehicles ---");
        List<IVehicle> vehicles = _handler.GetAllVehicles().ToList();

        if (!vehicles.Any())
        {
            _ui.WriteLine("The garage is currently empty.");
            return;
        }

        foreach (IVehicle vehicle in vehicles)
        {
            _ui.WriteLine(vehicle.GetVehicleInfo());
        }
    }

    private void ListVehicleTypesAndCounts()
    {
        _ui.WriteLine("\n--- Vehicle Types and Counts ---");
        List<(string VehicleType, int Count)> counts = _handler.GetVehicleTypeCounts().ToList();

        if (!counts.Any())
        {
            _ui.WriteLine("The garage is currently empty.");
            return;
        }

        foreach ((string vehicleType, int count) in counts)
        {
            _ui.WriteLine($"{vehicleType}: {count}");
        }
    }

    private void ParkVehicleMenu()
    {
        if (_handler.IsFull)
        {
            _ui.WriteLine("\nCannot park a new vehicle: The garage is already full.");
            return;
        }

        _ui.WriteLine("\n--- Select Vehicle Type to Park ---");
        _ui.WriteLine("1. Airplane");
        _ui.WriteLine("2. Motorcycle");
        _ui.WriteLine("3. Car");
        _ui.WriteLine("4. Bus");
        _ui.WriteLine("5. Boat");
        _ui.WriteLine("0. Back to main menu");
        _ui.Write("Choice: ");

        string typeChoice = _ui.ReadLine().Trim();
        if (typeChoice == "0")
            return;

        if (typeChoice != "1" && typeChoice != "2" && typeChoice != "3" && typeChoice != "4" && typeChoice != "5")
        {
            _ui.WriteLine("Invalid vehicle type selection.");
            return;
        }

        string regNo = PromptNonEmptyString("Enter registration number: ").ToUpperInvariant();
        if (_handler.FindByRegistrationNumber(regNo) is not null)
        {
            _ui.WriteLine($"Error: A vehicle with registration number '{regNo}' is already parked.");
            return;
        }

        string color = PromptNonEmptyString("Enter color: ");
        int wheels = PromptNonNegativeInt("Enter number of wheels: ");

        IVehicle? vehicle = typeChoice switch
        {
            "1" => new Airplane(regNo, color, wheels, PromptPositiveInt("Enter number of engines: ")),
            "2" => new Motorcycle(regNo, color, wheels, PromptPositiveDouble("Enter cylinder volume (cc): ")),
            "3" => new Car(regNo, color, wheels, PromptFuelType()),
            "4" => new Bus(regNo, color, wheels, PromptPositiveInt("Enter number of seats: ")),
            "5" => new Boat(regNo, color, wheels, PromptPositiveDouble("Enter boat length (meters): ")),
            _ => null
        };

        if (vehicle is not null)
        {
            (bool _, string message) = _handler.ParkVehicle(vehicle);
            _ui.WriteLine(message);
        }
    }

    private void UnparkVehicleMenu()
    {
        string regNo = PromptNonEmptyString("\nEnter registration number of the vehicle to unpark: ");
        (bool _, string message) = _handler.UnparkVehicle(regNo);
        _ui.WriteLine(message);
    }

    private void FindVehicleByRegistrationNumberMenu()
    {
        string regNo = PromptNonEmptyString("\nEnter registration number to search for: ");
        IVehicle? vehicle = _handler.FindByRegistrationNumber(regNo);

        if (vehicle is null)
        {
            _ui.WriteLine($"No vehicle found with registration number '{regNo.ToUpperInvariant()}'.");
        }
        else
        {
            _ui.WriteLine("Vehicle found:");
            _ui.WriteLine(vehicle.GetVehicleInfo());
        }
    }

    private void SearchVehiclesByPropertiesMenu()
    {
        _ui.WriteLine("\n--- Search Vehicles (Press Enter to skip a filter) ---");
        _ui.Write("Vehicle type (Airplane, Motorcycle, Car, Bus, Boat): ");
        string typeInput = _ui.ReadLine().Trim();
        string? vehicleType = string.IsNullOrWhiteSpace(typeInput) ? null : typeInput;

        _ui.Write("Color: ");
        string colorInput = _ui.ReadLine().Trim();
        string? color = string.IsNullOrWhiteSpace(colorInput) ? null : colorInput;

        int? wheels = PromptOptionalNonNegativeInt("Number of wheels: ");

        List<IVehicle> results = _handler.SearchVehicles(vehicleType, color, wheels).ToList();

        if (!results.Any())
        {
            _ui.WriteLine("No vehicles matched the specified criteria.");
            return;
        }

        _ui.WriteLine($"\nFound {results.Count} matching vehicle(s):");
        foreach (IVehicle vehicle in results)
        {
            _ui.WriteLine(vehicle.GetVehicleInfo());
        }
    }

    private string PromptNonEmptyString(string prompt)
    {
        while (true)
        {
            _ui.Write(prompt);
            string input = _ui.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(input))
                return input;

            _ui.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    private int PromptPositiveInt(string prompt)
    {
        while (true)
        {
            _ui.Write(prompt);
            if (int.TryParse(_ui.ReadLine().Trim(), out int value) && value > 0)
                return value;

            _ui.WriteLine("Invalid input. Please enter a positive whole number greater than 0.");
        }
    }

    private int PromptNonNegativeInt(string prompt)
    {
        while (true)
        {
            _ui.Write(prompt);
            if (int.TryParse(_ui.ReadLine().Trim(), out int value) && value >= 0)
                return value;

            _ui.WriteLine("Invalid input. Please enter a whole number (0 or greater).");
        }
    }

    private int? PromptOptionalNonNegativeInt(string prompt)
    {
        while (true)
        {
            _ui.Write(prompt);
            string input = _ui.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (int.TryParse(input, out int value) && value >= 0)
                return value;

            _ui.WriteLine("Invalid input. Please enter a whole number (0 or greater) or press Enter to skip.");
        }
    }

    private double PromptPositiveDouble(string prompt)
    {
        while (true)
        {
            _ui.Write(prompt);
            string input = _ui.ReadLine().Trim().Replace(',', '.');
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) && value > 0)
                return value;

            _ui.WriteLine("Invalid input. Please enter a positive number greater than 0.");
        }
    }

    private FuelType PromptFuelType()
    {
        while (true)
        {
            _ui.WriteLine("Select Fuel Type: 1. Gasoline | 2. Diesel | 3. Electric | 4. Hybrid");
            _ui.Write("Fuel choice (1-4): ");
            if (int.TryParse(_ui.ReadLine().Trim(), out int fuelValue) &&
                Enum.IsDefined(typeof(FuelType), fuelValue))
            {
                return (FuelType)fuelValue;
            }

            _ui.WriteLine("Invalid selection. Please enter a number between 1 and 4.");
        }
    }
}