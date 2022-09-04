using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models.AddModels;
using CourseProject.Web.ExceptionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

[Authorize]
[TypeFilter(typeof(ExceptionFilter))]
public class ItemsController : Controller
{
    private readonly IItemService _itemService;
    private readonly ICollectionService _collectionService;
    private readonly ICommentService _commentService;

    public ItemsController(IItemService itemService, ICollectionService collectionService,
        ICommentService commentService)
    {
        _itemService = itemService;
        _collectionService = collectionService;
        _commentService = commentService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string sortBy = "date", string sortOrder = "dsc", int page = 1)
    {
        var items = await _itemService.GetItems(sortBy, sortOrder, page);
        return View(items);
    }

    [AllowAnonymous]
    [Route("Items/{itemId:int}")]
    public async Task<IActionResult> Full(int itemId)
    {
        var item = await _itemService.GetItem(itemId);
        ViewBag.Comments = await _commentService.GetItemComments(itemId);
        return View(item);
    }

    public async Task<IActionResult> Add(int collectionId)
    {
        ViewBag.CollectionId = collectionId;
        ViewBag.CustomFieldsModel = (await _collectionService.GetCustomFieldsModel(collectionId))!;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddItemModel item)
    {
        if (!ModelState.IsValid) return View();
        var itemId = await _itemService.AddItem(item);
        return RedirectToAction("Full", "Items", new{itemId});
    }

    [Route("Items/{itemId:int}/Edit")]
    public async Task<IActionResult> Edit(int itemId)
    {
        var item = await _itemService.GetItem(itemId);
        ViewBag.Item = item;
        return View();
    }

    [HttpPost("Items/{itemId:int}/Edit")]
    public async Task<IActionResult> Edit(int itemId, AddItemModel item)
    {
        if (!ModelState.IsValid) return View();
        await _itemService.EditItem(itemId, item);
        return RedirectToAction("Full", "Items", new{itemId});
    }

    [HttpGet("Items/{itemId:int}/Delete")]
    public async Task<IActionResult> Delete(int itemId)
    {
        await _itemService.DeleteItem(itemId);
        return RedirectToAction("Index", "Items");
    }

    [HttpPost]
    public async Task<IActionResult> Like(int itemId)
    {
        var likesCount = await _itemService.LikeItem(itemId);
        return Json(likesCount);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Search(string searchString)
    {
        var items = await _itemService.SearchItems(searchString);
        return View(items);
    }
}