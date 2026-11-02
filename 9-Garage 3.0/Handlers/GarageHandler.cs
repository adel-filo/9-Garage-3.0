public class GarageHandler : IHandler
{
    private Garage<IVehicle>? _garage;

    public bool HasGarage => _garage is not null;
    public int Capacity => _garage?.Capacity ?? 0;
    public int Count => _garage?.Count ?? 0;
    public bool IsFull => _garage?.IsFull ?? true;

    public void CreateGarage(int capacity)
    {
        _garage = new Garage<IVehicle>(capacity);
    }

    public int SeedDefaultVehicles()
    {
        if (_garage is null)
            return 0;

        List<IVehicle> sampleVehicles = new List<IVehicle>
            {
                new Car("ABC123", "Black", 4, FuelType.Gasoline),
                new Motorcycle("MCY777", "Pink", 3, 750),
                new Airplane("FLY001", "White", 6, 2),
                new Bus("BUS900", "Red", 6, 50),
                new Boat("SEA555", "Blue", 0, 12.5)
            };

        int parkedCount = 0;
        foreach (IVehicle vehicle in sampleVehicles)
        {
            if (_garage.Park(vehicle))
            {
                parkedCount++;
            }
        }

        return parkedCount;
    }

    public (bool Success, string Message) ParkVehicle(IVehicle vehicle)
    {
        if (_garage is null)
            return (false, "No garage has been initialized.");

        if (_garage.IsFull)
            return (false, "Cannot park vehicle because the garage is full.");

        if (_garage.FindByRegistrationNumber(vehicle.RegistrationNumber) is not null)
            return (false, $"A vehicle with registration number '{vehicle.RegistrationNumber}' is already parked in the garage.");

        bool parked = _garage.Park(vehicle);
        return parked
            ? (true, $"{vehicle.GetType().Name} with registration number '{vehicle.RegistrationNumber}' has been parked successfully.")
            : (false, "Failed to park vehicle.");
    }

    public (bool Success, string Message) UnparkVehicle(string registrationNumber)
    {
        if (_garage is null)
            return (false, "No garage has been initialized.");

        bool removed = _garage.Unpark(registrationNumber);
        return removed
            ? (true, $"Vehicle with registration number '{registrationNumber.ToUpperInvariant()}' has been unparked.")
            : (false, $"No vehicle with registration number '{registrationNumber.ToUpperInvariant()}' was found in the garage.");
    }

    public IEnumerable<IVehicle> GetAllVehicles()
    {
        return _garage is null ? Enumerable.Empty<IVehicle>() : _garage.ToList();
    }

    public IEnumerable<(string VehicleType, int Count)> GetVehicleTypeCounts()
    {
        if (_garage is null)
            return Enumerable.Empty<(string, int)>();

        return _garage
            .GroupBy(v => v.GetType().Name)
            .Select(g => (VehicleType: g.Key, Count: g.Count()))
            .OrderBy(x => x.VehicleType)
            .ToList();
    }

    public IVehicle? FindByRegistrationNumber(string registrationNumber)
    {
        return _garage?.FindByRegistrationNumber(registrationNumber);
    }

    public IEnumerable<IVehicle> SearchVehicles(string? vehicleType, string? color, int? numberOfWheels)
    {
        if (_garage is null)
            return Enumerable.Empty<IVehicle>();

        IEnumerable<IVehicle> query = _garage;

        if (!string.IsNullOrWhiteSpace(vehicleType))
        {
            query = query.Where(v => string.Equals(v.GetType().Name, vehicleType.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            query = query.Where(v => string.Equals(v.Color, color.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (numberOfWheels.HasValue)
        {
            query = query.Where(v => v.NumberOfWheels == numberOfWheels.Value);
        }

        return query.ToList();
    }
}