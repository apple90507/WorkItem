using Microsoft.AspNetCore.Mvc;
using MyWorkItem.Services;
using MyWorkItem.ViewModels;
using MyWorkItem.Models;
using MyWorkItem.Exceptions;
using MyWorkItem.Filters;
namespace MyWorkItem.Controllers;
[AdminOnly]
[Route("admin/work-items")]
public class AdminWorkItemController : Controller
{
    private readonly WorkItemService _workItemService;
    private readonly ICurrentUserService _currentUserService;
    public AdminWorkItemController(WorkItemService workItemService,ICurrentUserService currentUserService)
    {
        _workItemService = workItemService;
        _currentUserService = currentUserService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var workItems=await _workItemService. GetAdminWorkItemsAsync();
        return View(workItems);
    }
    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var workItem=await _workItemService.GetEditWorkItemAsync(id);
        if (workItem==null){
            return NotFound(); 
        }
        ViewData["Id"] = id;
        return View(workItem);
    }
    [HttpPost("{id}/edit")]
    public async Task<IActionResult> Edit(int id,EditWorkItemViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Id"] = id;
            return View(viewModel);
        }
        try 
        {
            await _workItemService.EditWorkItemAsync(id,viewModel);
            TempData["SuccessMessage"]="更新成功";
        }
        catch(WorkItemNotFoundException ex)
        {
            TempData["ErrorMessage"]=ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("{id}/delete")]
    public async Task<IActionResult> Delete(int id){
        try 
        {
            await _workItemService.DeleteWorkItemAsync(id);
            TempData["SuccessMessage"]="刪除成功";
        }
        catch(WorkItemNotFoundException ex)
        {
            TempData["ErrorMessage"]=ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet("new")]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost("new")]
    public async Task<IActionResult> Create(CreateWorkItemViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }
        var workItem = new WorkItem
        {
            Title=viewModel.Title,
            Description=viewModel.Description
        };
            await _workItemService.CreateWorkItemAsync(workItem);
            TempData["SuccessMessage"]="新增成功";
            return RedirectToAction(nameof(Index));  
    }
}