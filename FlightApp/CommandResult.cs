public class CommandResult
{
    public CommandResult(IEnumerable<string> messages)
    {
        Messages = messages;
        QueryTable = null;
    }
    public CommandResult(QueryTableResult? queryTable = null)
    {
        Messages = Enumerable.Empty<string>();
        QueryTable = queryTable;
    }

    public IEnumerable<string> Messages { get; }

    public QueryTableResult? QueryTable { get; }
}

public record class QueryTableResult(IEnumerable<QueryTableColumn> Columns, IEnumerable<QueryTableRow> Rows);

public record class QueryTableRow(IDictionary<string, string> Cells);

public record class QueryTableColumn(string ColumnName);
