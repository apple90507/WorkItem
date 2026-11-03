using System.Net;
using Microsoft.Extensions.DependencyInjection;
using MyWorkItem.Data;
using MyWorkItem.Tests.Integration;

public class AdminAuthorizationTests
{
    [Fact]
    public async Task NonAdmin_CannotAccessAdminPage()
    {
        using var factory = new CustomWebApplicationFactory(isAdmin: false);
        factory.InitializeDatabase();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/admin/work-items");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_CannotAccessNonExistentWorkItem()
    {
        using var factory = new CustomWebApplicationFactory(isAdmin: true);
        factory.InitializeDatabase();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/admin/work-items/999/edit");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Admin_CanAccessExistingWorkItem()
    {
        using var factory = new CustomWebApplicationFactory(isAdmin: true);
        factory.InitializeDatabase();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var workItem = new WorkItem
        {
            Title = "Test Work Item",
        };
        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();

        var response = await client.GetAsync($"/admin/work-items/{workItem.Id}/edit");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(workItem.Id > 0);
    }
}
