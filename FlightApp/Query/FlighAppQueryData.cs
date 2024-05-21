using FlightApp.Query.Condition;

namespace FlightApp.Query
{
    internal enum QueryOperation
    {
        Add,
        Delete,
        Display,
        Update
    }

    internal enum QueryObjectClass
    {
       Airport,
       Flight,
       PassengerPlane,
       CargoPlane,
       Cargo,
       Passenger,
       Crew
    }

    internal record class FlighAppQueryData(
        QueryOperation Operation, 
        QueryObjectClass ObjectClass, 
        IList<ConditionToken>? Conditions, 
        IDictionary<string, string>? Values, 
        string[]? Fields);
}

