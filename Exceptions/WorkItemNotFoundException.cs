namespace MyWorkItem.Exceptions;
public class InvalidWorkItemTitleException:Exception
{
    public InvalidWorkItemTitleException(string message):base(message)
    {
        
    }
}
public class WorkItemNotFoundException:Exception
{
    public WorkItemNotFoundException(string message):base(message)
    {
        
    }
}

