using CourseProject.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    public async Task<IActionResult> Index(int page = 1, string userId = "")
    {
        var userPage = await _userService.GetUserPage(6 * page - 6, userId);
        return View(userPage);
    }
}