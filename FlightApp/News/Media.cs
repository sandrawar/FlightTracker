using FlightApp;
using FlightApp.News;

internal interface INewsReporter
{
    string Name { get; }
    string Report(IReportable reportable);
}

internal abstract class NewsReporter : INewsReporter
{
    protected NewsReporter(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract string Report(IReportable reportable);
}

internal class Radio : NewsReporter, INewsReporter
{
    public Radio(string name): base(name)
    {
    }

    public override string Report(IReportable reportable) =>
        reportable.ClassType switch
        {
            "AI" => $"Reporting for {Name}, Ladies and gentelmen, we are at the {reportable.Name} airport",
            "PP" => $"Reporting for {Name}, Ladies and gentelmen, we’ve just witnessed {reportable.Serial} take off.",
            "CP" => $"Reporting for {Name}, Ladies and gentelmen, we are seeing the {reportable.Serial} aircraft fly above us.",
            _ => string.Empty
        };
}


internal class Televison : NewsReporter, INewsReporter
{
    public Televison(string name) : base(name)
    {
    }

    public override string Report(IReportable reportable) =>
        reportable.ClassType switch
        {
            "AI" => $"<An image of {reportable.Name} airport>",
            "PP" => $"<An image of {reportable.Serial} passanger plane>",
            "CP" => $"<An image of {reportable.Serial} cargo plane>",
            _ => string.Empty
        };
}

internal class Newspaper : NewsReporter, INewsReporter
{
    public Newspaper(string name) : base(name)
    {
    }

    public override string Report(IReportable reportable) =>
        reportable.ClassType switch
        {
            "AI" => $"{Name} - A report from the {reportable.Name} airport, {reportable.Country}.",
            "PP" => $"{Name} - Breaking news! {reportable.Model} aircraft loses EASA fails certification after inspection of {reportable.Serial}",
            "CP" => $"{Name} - An interview with the crew of {reportable.Serial}.",
            _ => string.Empty
        };
}