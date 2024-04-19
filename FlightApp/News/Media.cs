using FlightApp;
using FlightApp.News;

internal interface INewsReporter
{
    string Name { get; }
    
    string Report(Airport airport);
    string Report(PassangerPlane passangerPlane);
    string Report(CargoPlane cargoPlane);

}

internal abstract class NewsReporter : INewsReporter
{
    protected NewsReporter(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract string Report(Airport airport);
    public abstract string Report(PassangerPlane passangerPlane);
    public abstract string Report(CargoPlane cargoPlane);
}

internal class Radio : NewsReporter, INewsReporter
{
    public Radio(string name): base(name)
    {
    }

    public override string Report(Airport airport) 
        => $"Reporting for {Name}, Ladies and gentelmen, we are at the {airport.Name} airport";

    public override string Report(PassangerPlane passangerPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we’ve just witnessed {passangerPlane.Serial} take off.";

    public override string Report(CargoPlane cargoPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we are seeing the {cargoPlane.Serial} aircraft fly above us.";
}


internal class Televison : NewsReporter, INewsReporter
{
    public Televison(string name) : base(name)
    {
    }

    public override string Report(Airport airport)
        => $"<An image of {airport.Name} airport>";

    public override string Report(PassangerPlane passangerPlane)
        => $"<An image of {passangerPlane.Serial} passanger plane>";

    public override string Report(CargoPlane cargoPlane)
        => $"<An image of {cargoPlane.Serial} cargo plane>";
}

internal class Newspaper : NewsReporter, INewsReporter
{
    public Newspaper(string name) : base(name)
    {
    }

    public override string Report(Airport airport)
        => $"{Name} - A report from the {airport.Name} airport, {airport.Country}.";

    public override string Report(PassangerPlane passangerPlane)
        => $"{Name} - Breaking news! {passangerPlane.Model} aircraft loses EASA fails certification after inspection of {passangerPlane.Serial}";

    public override string Report(CargoPlane cargoPlane)
        => $"{Name} - An interview with the crew of {cargoPlane.Serial}.";

}