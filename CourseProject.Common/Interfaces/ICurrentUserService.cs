using CourseProject.Dal.Entities;

namespace CourseProject.Common.Interfaces;

public interface ICurrentUserService
{
    string GetCurrentUserId();
    bool IsAdmin();
    bool IsCurrentUser(string userId);
    Task<User> GetCurrentUserAsync();
    User GetCurrentUser();
}