public class GarageTests
{
    [Fact]
    public void Constructor_ValidCapacity_SetsCapacityAndZeroCount()
    {
        int expectedCapacity = 5;

        Garage<IVehicle> garage = new Garage<IVehicle>(expectedCapacity);

        Assert.Equal(expectedCapacity, garage.Capacity);
        Assert.Equal(0, garage.Count);
        Assert.False(garage.IsFull);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_NonPositiveCapacity_ThrowsArgumentOutOfRangeException(int invalidCapacity)
    {
        Action act = () => new Garage<IVehicle>(invalidCapacity);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Park_AvailableSpaceAndUniqueVehicle_ReturnsTrueAndIncrementsCount()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(2);
        IVehicle car = new Car("ABC123", "Black", 4, FuelType.Gasoline);

        bool result = garage.Park(car);

        Assert.True(result);
        Assert.Equal(1, garage.Count);
    }

    [Fact]
    public void Park_GarageIsFull_ReturnsFalseAndDoesNotIncrementCount()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(1);
        garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
        IVehicle secondCar = new Car("XYZ999", "Red", 4, FuelType.Diesel);

        bool result = garage.Park(secondCar);

        Assert.False(result);
        Assert.Equal(1, garage.Count);
        Assert.True(garage.IsFull);
    }

    [Fact]
    public void Park_DuplicateRegistrationNumberCaseInsensitive_ReturnsFalse()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(3);
        garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));
        IVehicle duplicateCar = new Car("abC123", "Blue", 4, FuelType.Electric);

        bool result = garage.Park(duplicateCar);

        Assert.False(result);
        Assert.Equal(1, garage.Count);
    }

    [Fact]
    public void Unpark_ExistingRegistrationNumberCaseInsensitive_ReturnsTrueAndDecrementsCount()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(2);
        garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

        bool result = garage.Unpark("Abc123");

        Assert.True(result);
        Assert.Equal(0, garage.Count);
    }

    [Fact]
    public void Unpark_NonExistingRegistrationNumber_ReturnsFalse()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(2);
        garage.Park(new Car("ABC123", "Black", 4, FuelType.Gasoline));

        bool result = garage.Unpark("MISSING1");

        Assert.False(result);
        Assert.Equal(1, garage.Count);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("Abc123")]
    [InlineData("AbC123")]
    public void FindByRegistrationNumber_MatchingRegNoDifferentCasing_ReturnsVehicle(string searchRegNo)
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(2);
        IVehicle expectedVehicle = new Motorcycle("ABC123", "Pink", 3, 600);
        garage.Park(expectedVehicle);

        IVehicle? actualVehicle = garage.FindByRegistrationNumber(searchRegNo);

        Assert.NotNull(actualVehicle);
        Assert.Same(expectedVehicle, actualVehicle);
    }

    [Fact]
    public void GetEnumerator_MultipleVehiclesParkedAndOneUnparked_IteratesOnlyRemainingVehicles()
    {
        Garage<IVehicle> garage = new Garage<IVehicle>(3);
        IVehicle car = new Car("CAR111", "Black", 4, FuelType.Gasoline);
        IVehicle bus = new Bus("BUS222", "Red", 6, 45);
        IVehicle boat = new Boat("BOT333", "White", 0, 10.5);
        garage.Park(car);
        garage.Park(bus);
        garage.Park(boat);
        garage.Unpark("BUS222");

        List<IVehicle> parkedVehicles = garage.ToList();

        Assert.Equal(2, parkedVehicles.Count);
        Assert.Contains(car, parkedVehicles);
        Assert.Contains(boat, parkedVehicles);
        Assert.DoesNotContain(bus, parkedVehicles);
    }
}