namespace MyWorkItem.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAdmin { get; }
}