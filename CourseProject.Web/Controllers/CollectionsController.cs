using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models.AddModels;
using CourseProject.Web.ExceptionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseProject.Web.Controllers;

[Authorize]
[TypeFilter(typeof(ExceptionFilter))]
public class CollectionsController : Controller
{
    private const int PageSize = 9;
    
    private readonly ICollectionService _collectionService;
    private readonly ITopicService _topicService;

    private readonly IItemService _itemService;

    public CollectionsController(ICollectionService collectionService, ITopicService topicService,
        IItemService itemService)
    {
        _collectionService = collectionService;
        _topicService = topicService;
        _itemService = itemService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1)
    {
        var collections = await _collectionService.GetCollections(PageSize * page - PageSize);
        SetPagination(page,await _collectionService.GetCollectionsCount());
        return View(collections);
    }

    [AllowAnonymous]
    [Route("/Collections/{collectionId:int}")]
    public async Task<IActionResult> Full(int collectionId, int page = 1, string sortBy = "date", string sortOrder = "dsc")
    {
        var collection = await _collectionService.GetCollection(collectionId);
        collection.Items = await _itemService.GetItems(sortBy, sortOrder, page, collectionId);
        SetSortOrder(sortBy, sortOrder);
        SetPagination(page, await _itemService.GetItemsCountByCollection(collectionId));
        return View(collection);
    }

    public async Task<IActionResult> Add()
    {
        var topics = await _topicService.GetTopics();
        ViewBag.Topics = new SelectList(topics, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromForm] AddCollectionModel model)
    {
        if (!ModelState.IsValid) return View();
        var collectionId = await _collectionService.AddCollection(model);
        return RedirectToAction("Full", "Collections", new{collectionId});
    }

    [Route("Collections/{collectionId:int}/Edit/")]
    public async Task<IActionResult> Edit(int collectionId)
    {
        var collection = await _collectionService.GetCollection(collectionId);
        var topics = await _topicService.GetTopics();
        ViewBag.Topics = new SelectList(topics, "Id", "Name", (await _topicService.GetTopicByCollection(collectionId)).Id);
        ViewBag.Collection = collection;
        return View();
    }

    [HttpPost("Collections/{collectionId:int}/Edit/")]
    public async Task<IActionResult> Edit(int collectionId, [FromForm] AddCollectionModel model)
    {
        await _collectionService.EditCollection(collectionId, model);
        return RedirectToAction("Full", "Collections", new{collectionId});
    }

    [Route("Collections/{collectionId:int}/Delete/")]
    public async Task<IActionResult> Delete(int collectionId)
    {
        await _collectionService.DeleteCollection(collectionId);
        return View();
    }
    
    public async Task<FileContentResult> DownloadCsv(int collectionId)  
    {  
        var file = await _collectionService.GenerateCsv(collectionId);
        return File(file, "text/csv", "Collection.csv"); 
    } 

    private void SetPagination(int page, int count)
    {
        ViewBag.CurrentPage = page;
        ViewBag.PagesCount = Math.Ceiling(count / (double) PageSize);
    }
    
    private void SetSortOrder(string sortBy, string sortOrder)
    {
        ViewBag.SortBy = sortBy;
        ViewBag.SortOrder = sortOrder;
    }
}