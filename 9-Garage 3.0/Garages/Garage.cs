using System.Collections;

public class Garage<T> : IEnumerable<T> where T : IVehicle
{
    private readonly T?[] _vehicles;

    public int Capacity { get; }
    public int Count { get; private set; }
    public bool IsFull => Count >= Capacity;

    public Garage(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Garage capacity must be greater than zero.");

        Capacity = capacity;
        _vehicles = new T?[capacity];
    }

    public bool Park(T vehicle)
    {
        if (vehicle is null)
            throw new ArgumentNullException(nameof(vehicle));

        if (IsFull)
            return false;

        if (FindByRegistrationNumber(vehicle.RegistrationNumber) is not null)
            return false;

        for (int i = 0; i < _vehicles.Length; i++)
        {
            if (_vehicles[i] is null)
            {
                _vehicles[i] = vehicle;
                Count++;
                return true;
            }
        }

        return false;
    }

    public bool Unpark(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return false;

        for (int i = 0; i < _vehicles.Length; i++)
        {
            if (_vehicles[i] is not null &&
                string.Equals(_vehicles[i]!.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                _vehicles[i] = default;
                Count--;
                return true;
            }
        }

        return false;
    }

    public T? FindByRegistrationNumber(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return default;

        foreach (T? vehicle in _vehicles)
        {
            if (vehicle is not null &&
                string.Equals(vehicle.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return vehicle;
            }
        }

        return default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T? vehicle in _vehicles)
        {
            if (vehicle is not null)
            {
                yield return vehicle;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}