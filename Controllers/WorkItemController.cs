using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Services;
using MyWorkItem.ViewModels;
using MyWorkItem.Models;
using MyWorkItem.Exceptions;

namespace MyWorkItem.Controllers;
[Route("work-items")]
public class WorkItemController : Controller
{
    private readonly WorkItemService _workItemService;
    public WorkItemController(WorkItemService workItemService)
    {
        _workItemService = workItemService;
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? sort,int page = 1)
    {
        if (page <= 0)
        {
            return NotFound();
        }
        var workItems=await _workItemService.GetAllWorkItemsAsync(sort,page);
        if (page > Math.Max(1, workItems.TotalPages))
        {
            return NotFound();
        }
        ViewData["Sort"] = sort;
        return View(workItems);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Detail(int id, string? sort,int page=1)
    {
        var workItem=await _workItemService.GetWorkItemAsync(id);
        if (workItem==null){
            return NotFound(); 
        }
        ViewData["Sort"] = sort;
        ViewData["Page"] = page;
        return View(workItem);
    }
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(List<int> workItemIds,string? sort,int page=1)
    {
        try
        {
            var confirmedCount=await _workItemService.ConfirmWorkItemsAsync(workItemIds);
            TempData["SuccessMessage"]= $"已成功確認 {confirmedCount} 項目";
        }
        catch(WorkItemNotFoundException ex)
        {
            TempData["ErrorMessage"] =ex.Message;
        }
        return RedirectToAction(nameof(Index),new { sort=sort, page=page });
    }
    [HttpPost("undoconfirm")]
    public async Task<IActionResult> UndoConfirm(int workItemId,string? sort,int page=1)
    {
        try
        {
            await _workItemService.UndoConfirmationAsync(workItemId);
            TempData["SuccessMessage"]="已將1項目標記為待確認";
        }
        catch (WorkItemNotFoundException ex)
        {
            TempData["ErrorMessage"] =ex.Message;
        }
             return RedirectToAction(nameof(Index),new { sort=sort, page=page });
    }
}