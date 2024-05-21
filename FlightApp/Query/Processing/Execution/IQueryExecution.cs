namespace FlightApp.Query.Processing.Execution
{
    internal interface IQueryExecution<TObjectClass>
    {
        IEnumerable<TObjectClass> SelectSource();

        bool IsConditionMet(TObjectClass source);

        bool Update(TObjectClass source);

        bool Delete(TObjectClass source);

        CommandResult PrepareDisplayTable(IEnumerable<TObjectClass> source);

        CommandResult Add();
    }
}
