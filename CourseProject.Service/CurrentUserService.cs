using System.Security.Claims;
using CourseProject.Common.Interfaces;
using CourseProject.Dal.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public bool IsAdmin()
    {
        return _httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }

    public bool IsCurrentUser(string userId)
    {
        return GetCurrentUserId() == userId;
    }

    public async Task<User> GetCurrentUserAsync() =>
        await _userManager.Users.FirstOrDefaultAsync(u => u.Id == GetCurrentUserId()) ??
        throw new InvalidOperationException();

    public User GetCurrentUser()
    {
        return _userManager.Users.FirstOrDefault(u => u.Id == GetCurrentUserId()) ??
               throw new InvalidOperationException();
    }
}