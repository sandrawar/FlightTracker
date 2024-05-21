namespace FlightApp.Query.Condition
{
    internal interface IConditionEvaluator
    {
        bool Evaluate(Func<ConditionCompareOperation, string, IConstantNode, bool> conditionComparer);
    }

    internal class ConditionEvaluator : IConditionEvaluator
    {
        private readonly IConditionLogicNode? conditionNode;
        public readonly bool? constant;

        public ConditionEvaluator(IConditionLogicNode conditionTreeNode)
        {
            conditionNode = conditionTreeNode;
        }
        public ConditionEvaluator(bool constantEvaluation)
        {
            constant = constantEvaluation;
        }

        public bool Evaluate(Func<ConditionCompareOperation, string, IConstantNode, bool> conditionComparer) => 
            constant
            ?? (conditionNode is not null ? conditionNode.Evaluate(conditionComparer) : (bool?)null)
            ?? throw new InvalidOperationException();
    }


}
