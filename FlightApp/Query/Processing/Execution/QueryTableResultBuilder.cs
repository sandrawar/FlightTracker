namespace FlightApp.Query.Processing.Execution
{
    internal static class QueryTableResultBuilder
    {
        public static QueryTableResult BuildTable<TObjectClass>(
            string[] queryFields, 
            string[] syntaxFields,
            IEnumerable<TObjectClass> source,
            IDictionary<string, Func<TObjectClass, string>> sourceRead
            )
        {
            if (queryFields is null)
            {
                throw new InvalidOperationException("query fields not set");
            }

            var fieldsList = queryFields.Contains("*")
                ? syntaxFields
                : queryFields;

            var columns = fieldsList.Select(f => new QueryTableColumn(f));

            var rows = source.Select(
                cargo => new QueryTableRow(
                    fieldsList.Select(key => (key, sourceRead[key](cargo)))
                    .ToDictionary(f => f.key, f=>f.Item2)));

            return new QueryTableResult(columns.ToArray(), rows.ToArray());
        }
    }
}
