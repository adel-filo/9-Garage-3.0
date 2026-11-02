public interface IHandler
{
    bool HasGarage { get; }
    int Capacity { get; }
    int Count { get; }
    bool IsFull { get; }
    void CreateGarage(int capacity);
    int SeedDefaultVehicles();
    (bool Success, string Message) ParkVehicle(IVehicle vehicle);
    (bool Success, string Message) UnparkVehicle(string registrationNumber);
    IEnumerable<IVehicle> GetAllVehicles();
    IEnumerable<(string VehicleType, int Count)> GetVehicleTypeCounts();
    IVehicle? FindByRegistrationNumber(string registrationNumber);
    IEnumerable<IVehicle> SearchVehicles(string? vehicleType, string? color, int? numberOfWheels);
}