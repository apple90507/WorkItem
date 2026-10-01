public class UserWorkItem
{
    public int UserId { get; set; }
    public int WorkItemId { get; set; }
    public User User { get; set; }= null!;
    public WorkItem WorkItem { get; set; }= null!;
}