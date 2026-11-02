public class Motorcycle : Vehicle
{
    public double CylinderVolume { get; }

    public Motorcycle(string registrationNumber, string color, int numberOfWheels, double cylinderVolume)
        : base(registrationNumber, color, numberOfWheels)
    {
        if (cylinderVolume <= 0)
            throw new ArgumentOutOfRangeException(nameof(cylinderVolume), "Cylinder volume must be greater than zero.");

        CylinderVolume = cylinderVolume;
    }

    public override string GetVehicleInfo()
    {
        return $"{base.GetVehicleInfo()} | Cylinder Volume: {CylinderVolume} cc";
    }
}