using FlightApp.DataProcessor;
using FlightApp.Query.Processing.Execution;

namespace FlightApp.Query.Processing
{
    internal interface IFlightAppQueryProcessor
    {
        CommandResult Execute<TObjectClass>(QueryOperation operation, IQueryExecution<TObjectClass> queryExecution);
    }

    internal class FlightAppQueryProcessor : IFlightAppQueryProcessor
    {
        private readonly IFlightAppDataQueryRepository flightAppData;

        public FlightAppQueryProcessor(IFlightAppDataQueryRepository flightAppCompleteData)
        {
            flightAppData = flightAppCompleteData;
        }

        public CommandResult Execute<TObjectClass>(QueryOperation operation, IQueryExecution<TObjectClass> queryExecution)
        {
            

            var operationResult = operation switch
            {
                QueryOperation.Add => ProcessAdd(queryExecution),
                QueryOperation.Delete => ProcessDelete(queryExecution),
                QueryOperation.Display => ProcessDisplay(queryExecution),
                QueryOperation.Update => ProcessUpdate(queryExecution),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation.ToString(), "Operation not supported"),
            };

            return operationResult;

            //TODO: remove
            //return new CommandResult([
            //    .. operationResult.Messages,
            //    $"Execute{operation}: {queryExecution.GetType().Name}"]);
        }

        private CommandResult ProcessAdd<TObjectClass>(IQueryExecution<TObjectClass> queryExecution)
            => queryExecution.Add();

        private CommandResult ProcessDelete<TObjectClass>(IQueryExecution<TObjectClass> queryExecution)
        {
            var source = queryExecution.SelectSource(flightAppData);
            var selected = source.Where(queryExecution.IsConditionMet).ToArray();
            var deleted = selected.Select(queryExecution.Delete).ToArray();
            return new CommandResult([$"{deleted.Count()} objects deleted"]);
        }

        private CommandResult ProcessDisplay<TObjectClass>(IQueryExecution<TObjectClass> queryExecution)
        {
            var source = queryExecution.SelectSource(flightAppData);
            var selected = source.Where(queryExecution.IsConditionMet);
            return queryExecution.PrepareDisplayTable(selected);
        }

        private CommandResult ProcessUpdate<TObjectClass>(IQueryExecution<TObjectClass> queryExecution)
        {
            var source = queryExecution.SelectSource(flightAppData);
            var selected = source.Where(queryExecution.IsConditionMet);
            var updated = selected.Select(queryExecution.Update).ToArray();
            return new CommandResult([$"updated {updated.Count()} objects"]);
        }
    }
}
