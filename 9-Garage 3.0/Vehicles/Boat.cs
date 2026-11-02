public class Boat : Vehicle
{
    public double Length { get; }

    public Boat(string registrationNumber, string color, int numberOfWheels, double length)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");

        Length = length;
    }

    public override string GetVehicleInfo()
    {
        return $"{base.GetVehicleInfo()} | Length: {Length} m";
    }
}