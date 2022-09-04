using CourseProject.Domain.Abstractions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }
    [HttpPost]
    public IActionResult CultureManagement(string culture)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.MaxValue }
        );
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Index()
    {
        var homePage = await _homeService.GetHomeModel();
        return View(homePage);
    }
    
}

