namespace FlightApp.Query.Condition
{
    internal static class ConditionComparerHelper
    {
        public static bool Compare(string currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            operation switch
            {
                ConditionCompareOperation.Equal => currentValue == compareWith.RawValue,
                ConditionCompareOperation.NotEqual => currentValue != compareWith.RawValue,
                ConditionCompareOperation.LessOrEqual => string.CompareOrdinal(currentValue, compareWith.RawValue) <= 0,
                ConditionCompareOperation.GreaterThanOrEqual => string.CompareOrdinal(currentValue, compareWith.RawValue) >= 0,
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };

        public static bool Compare(ushort currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            operation switch
            {
                ConditionCompareOperation.Equal => currentValue == compareWith.ParseAsUshort(),
                ConditionCompareOperation.NotEqual => currentValue != compareWith.ParseAsUshort(),
                ConditionCompareOperation.LessOrEqual => currentValue <= compareWith.ParseAsUshort(),
                ConditionCompareOperation.GreaterThanOrEqual => currentValue >= compareWith.ParseAsUshort(),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };

        public static bool Compare(ulong currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            operation switch
            {
                ConditionCompareOperation.Equal => currentValue == compareWith.ParseAsUlong(),
                ConditionCompareOperation.NotEqual => currentValue != compareWith.ParseAsUlong(),
                ConditionCompareOperation.LessOrEqual => currentValue <= compareWith.ParseAsUlong(),
                ConditionCompareOperation.GreaterThanOrEqual => currentValue >= compareWith.ParseAsUlong(),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };

        public static bool Compare(float currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            operation switch
            {
                ConditionCompareOperation.Equal => currentValue == compareWith.ParseAsUlong(),
                ConditionCompareOperation.NotEqual => currentValue != compareWith.ParseAsUlong(),
                ConditionCompareOperation.LessOrEqual => currentValue <= compareWith.ParseAsUlong(),
                ConditionCompareOperation.GreaterThanOrEqual => currentValue >= compareWith.ParseAsUlong(),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };

        public static bool Compare(float? currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            currentValue.HasValue ? Compare(currentValue.Value, operation, compareWith) : false;

        public static bool Compare(DateTime currentValue, ConditionCompareOperation operation, IConstantNode compareWith) =>
            operation switch
            {
                ConditionCompareOperation.Equal => currentValue == compareWith.ParseAsDateTime(),
                ConditionCompareOperation.NotEqual => currentValue != compareWith.ParseAsDateTime(),
                ConditionCompareOperation.LessOrEqual => currentValue <= compareWith.ParseAsDateTime(),
                ConditionCompareOperation.GreaterThanOrEqual => currentValue >= compareWith.ParseAsDateTime(),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };
}
}
