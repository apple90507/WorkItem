using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MyWorkItem.Services;
using MyWorkItem.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyWorkItem.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
public class AdminAuthorizationTests
{
    [Fact]
    public async Task  NonAdmin_CannotAccessAdminPage()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ICurrentUserService>();
                services.AddScoped<ICurrentUserService>(_ => new FakeCurrentUserService(false));
            });
        });
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/admin/work-items");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    [Fact]
    public async Task  Admin_CannotAccessNonExistentWorkItem()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ICurrentUserService>();
                services.AddScoped<ICurrentUserService>(_ => new FakeCurrentUserService(true));
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });
            });
        });
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        var response = await client.GetAsync("/admin/work-items/999/edit");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task  Admin_CanAccessExistingWorkItem()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ICurrentUserService>();
                services.AddScoped<ICurrentUserService>(_ => new FakeCurrentUserService(true));
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });
            });
        });
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        var workItem = new WorkItem
        {
            Title = "Test Work Item",
        };
        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync();
        var response = await client.GetAsync("/admin/work-items/" + workItem.Id + "/edit");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(workItem.Id > 0);
    }
}
