using CourseProject.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

public class TagController : Controller
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("/Search/{searchString}")]
    public async Task<JsonResult> Search(string searchString)
    {
        var tags = await _tagService.SearchTags(searchString);
        return Json(tags);
    }
}