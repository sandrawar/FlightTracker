namespace FlightApp.News
{
    internal interface IReportable
    {
        string Report(INewsReporter newsReporter);
    }
}
