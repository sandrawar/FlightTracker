﻿using FlightApp;
using FlightApp.News;

internal interface INewsReporter
{
    string Name { get; }
    
    string Report(IAirport airport);
    string Report(IPassengerPlane passengerPlane);
    string Report(ICargoPlane cargoPlane);

}

internal abstract class NewsReporter : INewsReporter
{
    protected NewsReporter(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract string Report(IAirport airport);
    public abstract string Report(IPassengerPlane passengerPlane);
    public abstract string Report(ICargoPlane cargoPlane);
}

internal class Radio : NewsReporter, INewsReporter
{
    public Radio(string name): base(name)
    {
    }

    public override string Report(IAirport airport) 
        => $"Reporting for {Name}, Ladies and gentelmen, we are at the {airport.Name} airport";

    public override string Report(IPassengerPlane passengerPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we’ve just witnessed {passengerPlane.Serial} take off.";

    public override string Report(ICargoPlane cargoPlane)
        => $"Reporting for {Name}, Ladies and gentelmen, we are seeing the {cargoPlane.Serial} aircraft fly above us.";
}


internal class Televison : NewsReporter, INewsReporter
{
    public Televison(string name) : base(name)
    {
    }

    public override string Report(IAirport airport)
        => $"<An image of {airport.Name} airport>";

    public override string Report(IPassengerPlane passengerPlane)
        => $"<An image of {passengerPlane.Serial} passenger plane>";

    public override string Report(ICargoPlane cargoPlane)
        => $"<An image of {cargoPlane.Serial} cargo plane>";
}

internal class Newspaper : NewsReporter, INewsReporter
{
    public Newspaper(string name) : base(name)
    {
    }

    public override string Report(IAirport airport)
        => $"{Name} - A report from the {airport.Name} airport, {airport.Country}.";

    public override string Report(IPassengerPlane passengerPlane)
        => $"{Name} - Breaking news! {passengerPlane.Model} aircraft loses EASA fails certification after inspection of {passengerPlane.Serial}";

    public override string Report(ICargoPlane cargoPlane)
        => $"{Name} - An interview with the crew of {cargoPlane.Serial}.";

}