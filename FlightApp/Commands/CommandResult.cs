public class CommandResult
{
    public CommandResult(IEnumerable<string> messages)
    {
        Messages = messages;
    }

    public IEnumerable<string> Messages { get; }
}