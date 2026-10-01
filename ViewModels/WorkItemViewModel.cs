namespace MyWorkItem.ViewModels;
public class WorkItemViewModel
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsConfirmed { get; set; }
}