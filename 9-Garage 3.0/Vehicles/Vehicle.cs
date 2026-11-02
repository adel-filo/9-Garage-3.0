public abstract class Vehicle : IVehicle
{
    public string RegistrationNumber { get; }
    public string Color { get; }
    public int NumberOfWheels { get; }

    protected Vehicle(string registrationNumber, string color, int numberOfWheels)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            throw new ArgumentException("Registration number cannot be empty.", nameof(registrationNumber));

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Color cannot be empty.", nameof(color));

        if (numberOfWheels < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfWheels), "Number of wheels cannot be negative.");

        RegistrationNumber = registrationNumber.Trim().ToUpperInvariant();
        Color = color.Trim();
        NumberOfWheels = numberOfWheels;
    }

    public virtual string GetVehicleInfo()
    {
        return $"Type: {GetType().Name,-12} | RegNo: {RegistrationNumber,-8} | Color: {Color,-10} | Wheels: {NumberOfWheels,-3}";
    }
}