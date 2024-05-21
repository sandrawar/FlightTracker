using FlightApp.Query.Condition;
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
        private static class QueryGroupNames
        {
            public const string Operation = "operation";
            public const string Class = "class";
            public const string Conditions = "conditions";
            public const string Values = "values";
            public const string Fields = "fields";
        }
        private static class RegexPatterns
        {
            private const string patternOpAdd = $"(?<{QueryGroupNames.Operation}>^{QueryOperationName.Add})";
            private const string patternOpDelete = $"(?<{QueryGroupNames.Operation}>^{QueryOperationName.Delete})";
            private const string patternOpDisplay = $"(?<{QueryGroupNames.Operation}>^{QueryOperationName.Display})";
            private const string patternOpUpdate = $"(?<{QueryGroupNames.Operation}>^{QueryOperationName.Update})";
            private const string patternClass = @$"\s+(?<{QueryGroupNames.Class}>" +
                $"{QuerySyntax.Flight.ClassName}" +
                $"|{QuerySyntax.Airport.ClassName}" +
                $"|{QuerySyntax.PassengerPlane.ClassName}" +
                $"|{QuerySyntax.CargoPlane.ClassName}" +
                $"|{QuerySyntax.Cargo.ClassName}" +
                $"|{QuerySyntax.Passenger.ClassName}" +
                $"|{QuerySyntax.Crew.ClassName})";
            private const string patternConditions = @$"(\s+where\s+(?<{QueryGroupNames.Conditions}>.+))?";
            private const string patternValues = @$"\s+\((?<{QueryGroupNames.Values}>.+)\)";
            private const string patternFields = @$"\s+(?<{QueryGroupNames.Fields}>\*|[\w\.,\s+]+)";

            public const string PatternCommand = @$"{patternOpAdd}\s+.*$" +
                    @$"|{patternOpDelete}\s+.*$" +
                    @$"|{patternOpDisplay}\s+.*$" +
                    @$"|{patternOpUpdate}\s+.*$";

            public const string PatternQuery = @$"{patternOpAdd}{patternClass}\s+new{patternValues}\s*$" +
                @$"|{patternOpDelete}{patternClass}{patternConditions}\s*$" +
                @$"|{patternOpDisplay}{patternFields}\s+from{patternClass}{patternConditions}\s*$" +
                @$"|{patternOpUpdate}{patternClass}\s+set{patternValues}{patternConditions}\s*$";

            public const string PatternFieldSplit = @",\s*";
        }
        
        private Regex regexCommand;
        private Regex regexQuery;
        private Regex regexFieldSplit;

        public FlightAppQueryParser()
        {
            regexCommand = new Regex(RegexPatterns.PatternCommand, RegexOptions.Compiled);
            regexQuery = new Regex(RegexPatterns.PatternQuery, RegexOptions.Compiled);
            regexFieldSplit = new Regex(RegexPatterns.PatternFieldSplit, RegexOptions.Compiled);
        }

        public bool IsQueryCommand(string command)
            => regexCommand.IsMatch(command);

        public FlighAppQueryData? Parse(string command)
        {
            if (!TryMatchCommand(command, out var commandMatch)
                || !TryParseQueryOperation(commandMatch, out var queryOperation)
                || !TryParseQueryObjectClass(commandMatch, out var queryObjectClass)
                || !TryParseConditions(commandMatch, out var conditions)
                || !TryParseValues(commandMatch, out var values))
            {
                return null;
            }

            return new FlighAppQueryData(
                queryOperation,
                queryObjectClass,
                conditions,
                values,
                ParseFields(commandMatch)
            );
        }

        private bool TryMatchCommand(string command, out Match commandMatch)
        {
            commandMatch = regexQuery.Match(command);
            return commandMatch.Success;
        }

        private static bool TryParseQueryOperation(Match commandMatch, out QueryOperation queryOperation)
        {
            QueryOperation? parsedQueryOperation = commandMatch.Groups[QueryGroupNames.Operation].Value switch
            {
                QueryOperationName.Add => QueryOperation.Add,
                QueryOperationName.Delete => QueryOperation.Delete,
                QueryOperationName.Display => QueryOperation.Display,
                QueryOperationName.Update => QueryOperation.Update,
                _ => null
            };
            queryOperation = parsedQueryOperation ?? QueryOperation.Add;
            return parsedQueryOperation.HasValue;
        }

        private static bool TryParseQueryObjectClass(Match commandMatch, out QueryObjectClass queryObjectClass)
        {
            QueryObjectClass? parsedQueryObjectClass = commandMatch.Groups[QueryGroupNames.Class].Value switch
            {
                QuerySyntax.Airport.ClassName => QueryObjectClass.Airport,
                QuerySyntax.Flight.ClassName => QueryObjectClass.Flight,
                QuerySyntax.PassengerPlane.ClassName => QueryObjectClass.PassengerPlane,
                QuerySyntax.CargoPlane.ClassName => QueryObjectClass.CargoPlane,
                QuerySyntax.Cargo.ClassName => QueryObjectClass.Cargo,
                QuerySyntax.Passenger.ClassName => QueryObjectClass.Passenger,
                QuerySyntax.Crew.ClassName => QueryObjectClass.Crew,
                _ => null
            };
            queryObjectClass = parsedQueryObjectClass ?? QueryObjectClass.Airport;
            return parsedQueryObjectClass.HasValue;
        }

        private string[]? ParseFields(Match commandMatch)
        {
            var fields = commandMatch.Groups[QueryGroupNames.Fields]?.Value;
            var fieldlist = fields is not null
                ? regexFieldSplit.Split(fields)
                : null;
            return fieldlist;
        }

        private static bool TryParseConditions(Match commandMatch, out IList<ConditionToken>? conditionTokens)
        {
            var conditions = commandMatch.Groups[QueryGroupNames.Conditions]?.Value;
            
            if (string.IsNullOrEmpty(conditions))
            {
                conditionTokens = null;
                return true;
            }

            try
            {
                conditionTokens = ConditionTokenizer.Tokenize(conditions);
                return true;
            }
            catch (QueryProcessingException)
            {
                conditionTokens = null;
                return false;
            }
        }

        private static bool TryParseValues(Match commandMatch, out IDictionary<string, string>? values)
        {
            var rawValues = commandMatch.Groups[QueryGroupNames.Values]?.Value;
            
            if (string.IsNullOrEmpty(rawValues))
            {
                values = null;
                return true;
            }

            try
            {
                var pairlist = rawValues.Split(',');
                var setters = pairlist.Select(p => p.Split('='));
                if (setters.Any(v => v.Length != 2))
                {
                   throw new QueryProcessingException("Error parsing values");
                }

                values = setters.ToDictionary(v => v[0].Trim(), v => v[1].Trim());
                return true;
            }
            catch (Exception)
            {
                values = null;
                return false;
            }
        }
    }
}
