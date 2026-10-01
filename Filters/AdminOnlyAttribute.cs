using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyWorkItem.Services;
namespace MyWorkItem.Filters;

public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var currentUserService =context.HttpContext.RequestServices
        .GetRequiredService<ICurrentUserService>();
        if (!currentUserService.IsAdmin)
        {   
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}