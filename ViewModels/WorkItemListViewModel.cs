namespace MyWorkItem.ViewModels;

public class WorkItemListViewModel
{
    public List<WorkItemViewModel> Items { get; set; }= [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}