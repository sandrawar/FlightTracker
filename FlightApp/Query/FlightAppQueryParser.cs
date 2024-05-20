using System.Text.RegularExpressions;

namespace FlightApp.Query
{
    internal interface IFlightAppQueryParser
    {
        bool IsQueryCommand(string command);
        FlighAppQueryData? Parse(string command);
    }

    internal class FlightAppQueryParser : IFlightAppQueryParser
    {
        private static class QueryOperationName
        {
            public const string Add = "add";
            public const string Delete = "delete";
            public const string Display = "display";
            public const string Update = "update";
        }
        private static class QueryObjectClassName
        {
            public const string Airport = "Airport";
            public const string Flight = "Flight";
            public const string PassengerPlane = "PassengerPlane";
            public const string CargoPlane = "CargoPlane";
            public const string Cargo = "Cargo";
            public const string Passenger = "Passenger";
            public const string Crew = "Crew";
        }

        private const string regexOpAdd = $"(?<operation>^{QueryOperationName.Add})";
        private const string regexOpDelete = $"(?<operation>^{QueryOperationName.Delete})";
        private const string regexOpDisplay = $"(?<operation>^{QueryOperationName.Display})";
        private const string regexOpUpdate = $"(?<operation>^{QueryOperationName.Update})";
        private const string regexClass = @"\s+(?<class>" +
            $"{QueryObjectClassName.Flight}" +
            $"|{QueryObjectClassName.Airport}" +
            $"|{QueryObjectClassName.PassengerPlane}" +
            $"|{QueryObjectClassName.CargoPlane}" +
            $"|{QueryObjectClassName.Cargo}" +
            $"|{QueryObjectClassName.Passenger}" +
            $"|{QueryObjectClassName.Crew})";
        private const string regexConditions = @"(\s+where\s+(?<conditions>.+))?";
        private const string regexValues = @"\s+\((?<values>.+)\)";
        private const string regexFields = @"\s+(?<fields>\*|[\w,\s+]+)";

        private const string regexFieldSplit = @",\s*";
        private Regex commandRegex;
        private Regex queryRegex;
        private Regex fieldSplitRegex;

        public FlightAppQueryParser()
        {
            commandRegex = new Regex(@$"{regexOpAdd}\s+.*$" +
                @$"|{regexOpDelete}\s+.*$" +
                @$"|{regexOpDisplay}\s+.*$" +
                @$"|{regexOpUpdate}\s+.*$",
                RegexOptions.Compiled | RegexOptions.Multiline);

            queryRegex = new Regex(@$"{regexOpAdd}{regexClass}\s+new{regexValues}\s*$" +
                @$"|{regexOpDelete}{regexClass}{regexConditions}\s*$" +
                @$"|{regexOpDisplay}{regexFields}\s+from{regexClass}{regexConditions}\s*$" +
                @$"|{regexOpUpdate}{regexClass}\s+set{regexValues}{regexConditions}\s*$",
                RegexOptions.Compiled | RegexOptions.Multiline);

            fieldSplitRegex = new Regex(regexFieldSplit, RegexOptions.Compiled);
        }

        public bool IsQueryCommand(string command)
            => commandRegex.IsMatch(command);

        public FlighAppQueryData? Parse(string command)
        {
            var commandMatch = queryRegex.Match(command);
            if (!commandMatch.Success)
            {
                return null;
            }

            QueryOperation? queryOperation = commandMatch.Groups["operation"].Value switch
            {
                QueryOperationName.Add => QueryOperation.Add,
                QueryOperationName.Delete => QueryOperation.Delete,
                QueryOperationName.Display => QueryOperation.Display,
                QueryOperationName.Update => QueryOperation.Update,
                _ => null
            };
            if (!queryOperation.HasValue)
            {
                return null;
            }

            QueryObjectClass? queryObjectClass = commandMatch.Groups["class"].Value switch
            {
                QueryObjectClassName.Airport => QueryObjectClass.Airport,
                QueryObjectClassName.Flight => QueryObjectClass.Flight,
                QueryObjectClassName.PassengerPlane => QueryObjectClass.PassengerPlane,
                QueryObjectClassName.CargoPlane => QueryObjectClass.CargoPlane,
                QueryObjectClassName.Cargo => QueryObjectClass.Cargo,
                QueryObjectClassName.Passenger => QueryObjectClass.Passenger,
                QueryObjectClassName.Crew => QueryObjectClass.Crew,
                _ => null
            };
            if (!queryObjectClass.HasValue)
            {
                return null;
            }

            var fields = commandMatch.Groups["fields"]?.Value;
            var fieldlist = fields is not null
                ? fieldSplitRegex.Split(fields)
                : null;

            return new FlighAppQueryData(
                queryOperation.Value,
                queryObjectClass.Value,
                commandMatch.Groups["conditions"]?.Value,
                commandMatch.Groups["values"]?.Value,
                fieldlist
            );
        }
    }
}
