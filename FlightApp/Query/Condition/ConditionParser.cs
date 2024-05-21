namespace FlightApp.Query.Condition
{
    internal class ConditionParser
    {
        private IList<ConditionToken> tokens;
        private int position;

        public ConditionParser(IList<ConditionToken> conditionTokens)
        {
            tokens = conditionTokens;
            position = 0;
        }

        public IConditionEvaluator Parse() => new ConditionEvaluator(ParseOrExpression());

        private IConditionLogicNode ParseOrExpression()
        {
            var left = ParseAndExpression();

            while (Match("or"))
            {
                var operatorToken = Previous();
                var right = ParseAndExpression();
                left = new ConditionBinaryLogicalOperationNode(left, right, operatorToken.Value);
            }

            return left;
        }

        private IConditionLogicNode ParseAndExpression()
        {
            var left = ParseEqualityExpression();

            while (Match("and"))
            {
                var operatorToken = Previous();
                var right = ParseEqualityExpression();
                left = new ConditionBinaryLogicalOperationNode(left, right, operatorToken.Value);
            }

            return left;
        }

        private IConditionLogicNode ParseEqualityExpression()
        {
            var left = ParseIdentifier();

            while (Match("=", "!=", "<=", ">=", ">", "<"))
            {
                var operatorToken = Previous();
                var right = ParseConstant();
                return new ConditionComparisonOperationNode(left, right, operatorToken.Value);
            }

            throw new QueryProcessingException($"Expected token not found");
        }

        private IIdentifierNode ParseIdentifier()
        {
            if (Match(ConditionTokenType.Identifier))
            {
                return new IdentifierNode(Previous().Value);
            }

            throw new QueryProcessingException("Unexpected token.");
        }
        private IConstantNode ParseConstant()
        {
            if (Match(ConditionTokenType.Number))
            {
                return new ConstantNode(Previous().Value);
            }

            if (Match(ConditionTokenType.String))
            {
                return new ConstantNode(Previous().Value);
            }

            throw new QueryProcessingException("Unexpected token.");
        }

        private bool Match(params string[] types)
        {
            if (types.Any(Check))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Match(ConditionTokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Check(string type) => !IsAtEnd() && tokens[position].Value == type;

        private bool Check(ConditionTokenType type) => !IsAtEnd() && tokens[position].TokenType == type;

        private ConditionToken Advance()
        {
            if (!IsAtEnd())
            { 
                position++;
            }
            return Previous();
        }

        private bool IsAtEnd() => tokens[position].TokenType == ConditionTokenType.End;

        private ConditionToken Previous() => tokens[position - 1];
    }
}
