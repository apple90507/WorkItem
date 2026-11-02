using MyWorkItem.Services;

namespace MyWorkItem.Tests.Fakes;

public class FakeCurrentUserService :ICurrentUserService
{
    private readonly bool _isAdmin;
    
    public FakeCurrentUserService(bool isAdmin)
    {
        _isAdmin = isAdmin;
    }

    public int UserId => 1;
    public bool IsAdmin => _isAdmin;
}