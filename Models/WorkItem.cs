public class WorkItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt{get;set;}
    public DateTime UpdatedAt{get;set;}
}