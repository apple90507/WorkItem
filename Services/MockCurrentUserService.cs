namespace MyWorkItem.Services;

public class MockCurrentUserService :ICurrentUserService
{
    public int UserId => 1;
    public bool IsAdmin => false;
}