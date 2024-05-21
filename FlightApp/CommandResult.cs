public class CommandResult
{
    public CommandResult(IReadOnlyCollection<string> messages)
    {
        Messages = messages;
        QueryTable = null;
    }
    public CommandResult(QueryTableResult? queryTable = null)
    {
        Messages = [];
        QueryTable = queryTable;
    }

    public IReadOnlyCollection<string> Messages { get; }

    public QueryTableResult? QueryTable { get; }
}

public record class QueryTableResult(IReadOnlyCollection<QueryTableColumn> Columns, IReadOnlyCollection<QueryTableRow> Rows);

public record class QueryTableRow(IDictionary<string, string> Cells);

public record class QueryTableColumn(string ColumnName);
