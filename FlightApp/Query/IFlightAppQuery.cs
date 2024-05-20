namespace FlightApp.Query
{
    internal interface IFlightAppQuery
    {
        CommandResult Execute(FlighAppQueryData query);
    }

}
