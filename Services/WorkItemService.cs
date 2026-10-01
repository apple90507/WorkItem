using MyWorkItem.Data;
using Microsoft.EntityFrameworkCore;
using MyWorkItem.Exceptions;
using MyWorkItem.ViewModels;
namespace MyWorkItem.Services;

public class WorkItemService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService  _currentUserService;
    public WorkItemService(
        AppDbContext db ,
        ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUserService=currentUserService;
    }
    public async Task<WorkItemListViewModel> GetAllWorkItemsAsync(string? sort,int page)
    {   
        var pagesize=5;
        var query=_db.WorkItems.AsQueryable();
        if(sort=="asc")
        {
            query=query.OrderBy(x=>x.CreatedAt);
        }    
        else
        {
            query=query.OrderByDescending(x=>x.CreatedAt);
        }
        var items= await query
            .Skip((page-1)*pagesize)
            .Take(pagesize)
            .Select(x=>new WorkItemViewModel 
            {
                Id=x.Id,
                Title=x.Title,
                IsConfirmed= _db.UserWorkItems.Any(
                uw => uw.UserId==_currentUserService.UserId && uw.WorkItemId==x.Id)
            })
            .ToListAsync();
        var totalcount=await _db.WorkItems.CountAsync();
        var totalPages=(int)Math.Ceiling(totalcount/(double)pagesize);
        var workItemListViewModel=new WorkItemListViewModel{
            Items=items,
            CurrentPage=page,
            TotalPages=totalPages
        };
        return workItemListViewModel;
    }
    public async Task<WorkItemDetailViewModel?> GetWorkItemAsync(int id)
    {
        return await _db.WorkItems
        .Where(x => x.Id == id)
        .Select(x=>new WorkItemDetailViewModel
        {
            Id =x.Id,
            Title=x.Title,
            IsConfirmed= _db.UserWorkItems.Any(
                uw => uw.UserId==_currentUserService.UserId && uw.WorkItemId==x.Id),
            CreatedAt=x.CreatedAt,
            UpdatedAt=x.UpdatedAt,
            Description=x.Description
        })
        .FirstOrDefaultAsync();
    }
    public async Task CreateWorkItemAsync(WorkItem workItem)
    {
        if(string.IsNullOrWhiteSpace(workItem.Title)){
            throw new InvalidWorkItemTitleException("Title cannot be empty.");
        }
        workItem.Title = workItem.Title.Trim();
        var now=DateTime.UtcNow;
        workItem.CreatedAt=now;
        workItem.UpdatedAt=now;
        _db.WorkItems.Add(workItem);
        await _db.SaveChangesAsync();
    }
    public async Task<int> ConfirmWorkItemsAsync(List<int> workItemIds)
    {
        var existing = await _db.UserWorkItems
            .Where(uw => uw.UserId==_currentUserService.UserId && workItemIds.Contains(uw.WorkItemId))
            .ToListAsync();
        var distinctIds = workItemIds.Distinct().ToList();
        var existingId=await _db.WorkItems
            .Where(uw=>distinctIds.Contains(uw.Id))
            .ToListAsync();
        if (distinctIds.Count!=existingId.Count)
        {
            throw new WorkItemNotFoundException("更新失敗");
        }
        var newIds = workItemIds
            .Where(id => !existing.Any(uw => id==uw.WorkItemId))
            .ToList();
        var newUserWorkItems = newIds
            .Select(id => new UserWorkItem
            {
                UserId=_currentUserService.UserId,
                WorkItemId=id
            })
            .ToList();
        _db.UserWorkItems.AddRange(newUserWorkItems);
        await _db.SaveChangesAsync();
        return newIds.Count;
    }
    public async Task  UndoConfirmationAsync(int workItemId)
    {
        var existingId=await _db.WorkItems
            .AnyAsync(uw=>workItemId==uw.Id);
        if (!existingId)
        {
            throw new WorkItemNotFoundException("更新失敗");
        }
        var existing = await _db.UserWorkItems
            .FirstOrDefaultAsync(
                uw => uw.UserId==_currentUserService.UserId && workItemId==uw.WorkItemId);
        
        if (existing != null)
        {
            _db.UserWorkItems.Remove(existing);
            await _db.SaveChangesAsync();
        }
            
    }
    public async Task<EditWorkItemViewModel?> GetEditWorkItemAsync(int id){
        return await _db.WorkItems
        .Where(x => x.Id == id)
        .Select(x=>new EditWorkItemViewModel
        {
            Title=x.Title,
            Description=x.Description
        })
        .FirstOrDefaultAsync();
    }
    public async Task EditWorkItemAsync(int id,EditWorkItemViewModel viewModel)
    {
        var workItem=await _db.WorkItems
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();
        if (workItem==null)
        {
            throw new WorkItemNotFoundException("修改失敗");
        }
        workItem.Title=viewModel.Title.Trim();
        workItem.Description=viewModel.Description;
        workItem.UpdatedAt=DateTime.UtcNow;
        var existings = await _db.UserWorkItems
            .Where(uw => workItem.Id==uw.WorkItemId)
            .ToListAsync();
        _db.UserWorkItems.RemoveRange(existings);    
        await _db.SaveChangesAsync();
    }
    public async Task<List<AdminWorkItemViewModel>> GetAdminWorkItemsAsync()
    {
        var items= await _db.WorkItems
            .OrderByDescending(x => x.CreatedAt)
            .Select(x=>new AdminWorkItemViewModel 
            {
                Id=x.Id,
                Title=x.Title
            })
            .ToListAsync();
        return items;
    }
    public async Task DeleteWorkItemAsync(int id){
        var workItem=await _db.WorkItems
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();
        if (workItem==null)
        {
            throw new WorkItemNotFoundException("刪除失敗");
        }
        _db.WorkItems.Remove(workItem);
        await _db.SaveChangesAsync();
    }
}