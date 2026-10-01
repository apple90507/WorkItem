using  System.ComponentModel.DataAnnotations;
namespace MyWorkItem.ViewModels;
public class EditWorkItemViewModel
{
    [Required]
    public required string Title{get;set;}
    public string? Description{get;set;}
}