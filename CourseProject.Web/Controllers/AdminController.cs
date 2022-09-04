using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AdminController(IAdminService adminService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _adminService = adminService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetUsers();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUsers(string[] userIds)
    {
        await _adminService.DeleteUsers(userIds);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserStatus(string[] userIds, bool isActive)
    {
        await _adminService.ChangeUserStatus(userIds, isActive);
        if (!isActive)
        {
            await SignOutIfCurrentUserChanged(userIds);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SetRole(string[] userIds, string role)
    {
        await _adminService.SetRole(userIds, role);
        return Ok();
    }

    private async Task SignOutIfCurrentUserChanged(string[] userIds)
    {
        var user = await _userManager.GetUserAsync(User);
        if (userIds.Contains(user.Id))
        {
            await _signInManager.SignOutAsync();
        }
    }
}