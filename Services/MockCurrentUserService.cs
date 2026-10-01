namespace MyWorkItem.Services;

public class MockCurrentUserService :ICurrentUserService
{
    public int UserId => 2;
    public bool IsAdmin => true;
}