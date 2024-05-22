using System.Globalization;
using System.Xml.Linq;

namespace FlightApp.Query.Condition
{
    internal interface IConditionLogicNode
    {
        bool Evaluate(Func<ConditionCompareOperation, string, IConstantNode, bool> conditionComparer);
    }

    internal enum ConditionLogicOperation
    {
        And,
        Or,
    }

    internal enum ConditionCompareOperation
    {
        Equal,
        NotEqual,
        LessOrEqual,
        GreaterThanOrEqual,
    }

    internal class ConditionBinaryLogicalOperationNode : IConditionLogicNode
    {
        private readonly IConditionLogicNode left;
        private readonly IConditionLogicNode right;
        private readonly ConditionLogicOperation operation;

        public ConditionBinaryLogicalOperationNode(IConditionLogicNode leftNode, IConditionLogicNode rightNode, string operatorSymbol)
        {
            left = leftNode;
            right = rightNode;
            operation = operatorSymbol switch
            {
                "and" => ConditionLogicOperation.And,
                "or" => ConditionLogicOperation.Or,
                _ => throw new QueryProcessingException($"Unknown operator: {operatorSymbol}")
            };
        }

       public bool Evaluate(Func<ConditionCompareOperation, string, IConstantNode, bool> conditionComparer)
        {
            var leftValue = left.Evaluate(conditionComparer);
            var rightValue = right.Evaluate(conditionComparer);

            return operation switch
            {
                ConditionLogicOperation.And => leftValue && rightValue,
                ConditionLogicOperation.Or => leftValue || rightValue,
                _ => throw new InvalidOperationException()
            };
        }
    }

    internal class ConditionComparisonOperationNode : IConditionLogicNode
    {
        private readonly IIdentifierNode left;
        private readonly IConstantNode right;
        private readonly ConditionCompareOperation operation;

        public ConditionComparisonOperationNode(IIdentifierNode leftNode, IConstantNode rightNode, string operatorSymbol)
        {
            left = leftNode;
            right = rightNode;
            operation = operatorSymbol switch
            {
                "=" => ConditionCompareOperation.Equal,
                "!=" => ConditionCompareOperation.NotEqual,
                ">=" => ConditionCompareOperation.GreaterThanOrEqual,
                "<=" => ConditionCompareOperation.LessOrEqual,
                _ => throw new QueryProcessingException($"Unknown operator: {operatorSymbol}")
            };
        }

        public bool Evaluate(Func<ConditionCompareOperation, string, IConstantNode, bool> conditionComparer) =>
            conditionComparer(operation, left.Name, right);
    }

    internal interface IIdentifierNode
    {
        string Name { get; }
    }

    internal class IdentifierNode : IIdentifierNode
    {
        public string Name { get; }

        public IdentifierNode(string identifierName)
        {
            Name = identifierName;
        }
    }

    internal interface IConstantNode
    {
        string RawValue { get; }
        ushort ParseAsUshort();
        ulong ParseAsUlong();
        float ParseAsFloat();
        DateTime ParseAsDateTime();
    }

    internal class ConstantNode : IConstantNode
    {
        private Lazy<ushort> asUshortLazy;
        private Lazy<ulong> asUlongLazy;
        private Lazy<float> asFloatLazy;
        private Lazy<DateTime> asDateTimeLazy;

        public ConstantNode(string rawValue)
        {
            RawValue = rawValue;
            asUshortLazy = new (() => Parse(ushort.Parse), LazyThreadSafetyMode.ExecutionAndPublication);
            asUlongLazy = new (() => Parse(ulong.Parse), LazyThreadSafetyMode.ExecutionAndPublication);
            asFloatLazy = new (() => Parse(float.Parse), LazyThreadSafetyMode.ExecutionAndPublication);
            asDateTimeLazy = new (() => Parse(DateTime.Parse), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private TResult Parse<TResult>(Func<string, IFormatProvider?,TResult> parse)
        {
            try
            {
                return parse(RawValue, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new QueryProcessingException($"Unbale to convert {RawValue} to {typeof(TResult)}");
            }
        }

        public string RawValue { get; }

        public ushort ParseAsUshort() => asUshortLazy.Value;

        public ulong ParseAsUlong() => asUlongLazy.Value;

        public float ParseAsFloat() => asFloatLazy.Value;

        public DateTime ParseAsDateTime() => asDateTimeLazy.Value;
    }
}
