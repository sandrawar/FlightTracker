
using System.Text.RegularExpressions;

namespace FlightApp.Query.Condition
{
    internal enum ConditionTokenType
    {
        Identifier,
        Logical,
        Operator,
        Number,
        String,
        End
    }

    internal record class ConditionToken(ConditionTokenType TokenType, string Value);

    internal class ConditionTokenizer
    {
        private const string patternLogical = @"and|or";
        private const string patternOperator = @"=|!=|<=|>=";
        private const string patternNumber = @"\d+(\.\d+)?";
        private const string patternIdOrValue = @"[A-Za-z_][\w\.]*";

        private static readonly Regex TokenRegex = new Regex(@$"\s*({patternLogical}|{patternOperator}|{patternNumber}|{patternIdOrValue})\s*|.+");

        public static List<ConditionToken> Tokenize(string expression)
        {
            var tokens = new List<ConditionToken>();
            var matches = TokenRegex.Matches(expression);

            ConditionTokenType previousTokenType = ConditionTokenType.End;
            foreach (Match match in matches)
            {
                string value = match.Groups[1].Value;
                ConditionTokenType tokenType = DetermineTokenType(previousTokenType, value);
                previousTokenType = tokenType;
                tokens.Add(new ConditionToken(tokenType, value));
            }
            tokens.Add(new ConditionToken(ConditionTokenType.End, string.Empty));

            return tokens;
        }

        private static ConditionTokenType DetermineTokenType(ConditionTokenType previousTokenType, string value)
        {
            if (Regex.IsMatch(value, $"^{patternLogical}$")) return ConditionTokenType.Logical;
            if (Regex.IsMatch(value, $"^{patternOperator}$")) return ConditionTokenType.Operator;
            if (Regex.IsMatch(value, $"^{patternNumber}$")) return ConditionTokenType.Number;
            if (Regex.IsMatch(value, $"^{patternIdOrValue}$")) return previousTokenType == ConditionTokenType.Operator ? ConditionTokenType.String : ConditionTokenType.Identifier;
            throw new QueryProcessingException($"Unrecognized token: {value}");
        }
    }
}
