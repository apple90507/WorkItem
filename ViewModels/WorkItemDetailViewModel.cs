namespace MyWorkItem.ViewModels;
public class WorkItemDetailViewModel
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}
    public string? Description {get; set;}
}