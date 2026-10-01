using Microsoft.EntityFrameworkCore;

namespace MyWorkItem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public DbSet<WorkItem> WorkItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserWorkItem> UserWorkItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserWorkItem>()
        .HasKey(x => new { x.UserId, x.WorkItemId });
        modelBuilder.Entity<UserWorkItem>()
        .HasOne(x => x.User)
        .WithMany()
        .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<UserWorkItem>()
        .HasOne(x => x.WorkItem)
        .WithMany()
        .HasForeignKey(x => x.WorkItemId);
        modelBuilder.Entity<User>().HasData(
            new User{
                Id = 1,
                Username = "Amy",
                IsAdmin = false
            },
            new User{
                Id = 2,
                Username = "Admin",
                IsAdmin = true
            },
            new User{
                Id = 3,
                Username = "Bob",
                IsAdmin = false
            }
        );
    }
}