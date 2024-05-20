namespace FlightApp.Command
{
    internal interface IFlighAppCommand
    {
        CommandResult Execute(string command);
    }
}
