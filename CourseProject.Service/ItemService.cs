using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class ItemService : IItemService
{
    private readonly ApplicationContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISearchService _searchService;
    private readonly IMapper _mapper;

    public ItemService(ApplicationContext context, ICurrentUserService currentUserService, ISearchService searchService,
        IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _searchService = searchService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemModel>> SearchItems(string searchString)
    {
        var idRange = await _searchService.SearchAsync(searchString);
        return _mapper.Map<IEnumerable<ItemModel>>(await _context.Items.Include(i => i.Tags).Include(i => i.User)
            .Include(i => i.Collection)
            .Where(x => idRange.Contains(x.Id)).ToListAsync());
    }
    
    public async Task<int> LikeItem(int itemId)
    {
        var item = await _context.Items.Include(i => i.UsersLikes).FirstOrDefaultAsync(x => x.Id == itemId);
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        AddOrRemoveUserLike(item ?? throw new KeyNotFoundException(), currentUser);
        await _context.SaveChangesAsync();
        return item.UsersLikes.Count;
    }

    private void AddOrRemoveUserLike(Item item, User user)
    {
        if (item.UsersLikes.Any(i => i.Id == user.Id))
        {
            item.UsersLikes.Remove(user);
        }
        else
        {
            item.UsersLikes.Add(user);
        }
    }
    
    public async Task<IEnumerable<ItemModel>?> GetItems(string sortBy, string sortOrder, int page = 0,
        int collectionId = 0, int pageSize = 9)
    {
        var query = GetItemsQueryOrder(sortBy, sortOrder);
        return collectionId == 0
            ? _mapper.Map<IEnumerable<ItemModel>>(await query.Skip(pageSize * page - pageSize).Take(pageSize)
                .Include(i => i.Tags).Include(i => i.Collection).ToListAsync())
            : _mapper.Map<IEnumerable<ItemModel>>(await query.Skip(pageSize * page - pageSize).Take(pageSize)
                .Include(i => i.Tags).Include(i => i.Collection).Where(i => i.Collection.Id == collectionId)
                .ToListAsync());
    }

    private IQueryable<Item> GetItemsQueryOrder(string sortBy, string sortOrder)
    {
        return sortBy switch
        {
            "name" => sortOrder == "asc"
                ? _context.Items.OrderBy(i => i.Name)
                : _context.Items.OrderByDescending(i => i.Name),
            "date" => sortOrder == "asc"
                ? _context.Items.OrderBy(i => i.CreatedDate)
                : _context.Items.OrderByDescending(i => i.CreatedDate),
            "likes" => sortOrder == "asc"
                ? _context.Items.OrderBy(i => i.UsersLikes.Count)
                : _context.Items.OrderByDescending(i => i.UsersLikes.Count),
            _ => _context.Items.OrderByDescending(i => i.CreatedDate)
        };
    }
    
    public async Task<int> GetItemsCountByCollection(int collectionId)
    {
        return await _context.Items.CountAsync(i => i.Collection.Id == collectionId);
    }

    public async Task<ItemModel> GetItem(int itemId)
    {
        return _mapper.Map<ItemModel>(await _context.Items.Include(i => i.Tags).Include(i => i.UsersLikes)
            .Include(i => i.Collection).Where(i => i.Id == itemId).FirstOrDefaultAsync()) ??
               throw new KeyNotFoundException("Item not found");
    }

    public async Task<int> AddItem(AddItemModel itemModel)
    {
        var item = await CreateItem(itemModel);
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        await AddItemToSearchIndex(item);
        return item.Id;
    }

    private async Task AddItemToSearchIndex(Item item)
    {
        var itemSearch = _mapper.Map<ItemSearchModel>(item);
        await _searchService.AddToIndexAsync(itemSearch);
    }

    private async Task<Item> CreateItem(AddItemModel itemModel)
    {
        var item = _mapper.Map<Item>(itemModel);
        item.Tags = await GetTags(itemModel.Tags);
        item.Collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == itemModel.CollectionId) ??
                          throw new KeyNotFoundException("Collection not found");
        return item;
    }

    public async Task EditItem(int itemId, AddItemModel itemModel)
    {
        var item = await _context.Items.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == itemId);
        CheckItem(item);
        await ChangeItem(item, itemModel);
        await AddItemToSearchIndex(item);
        await _context.SaveChangesAsync();
    }

    private void CheckItem(Item item)
    {
        CheckForNull(item);
        CheckForAccess(item);
    }

    private void CheckForNull(Item item)
    {
        if (item == null) throw new KeyNotFoundException("Item not found");
    }

    private void CheckForAccess(Item item)
    {
        if (!_currentUserService.IsCurrentUser(item.User.Id) && !_currentUserService.IsAdmin())
            throw new UnauthorizedAccessException("You are not owner of this item");
    }

    private async Task ChangeItem(Item item, AddItemModel itemModel)
    {
        item.Name = itemModel.Name;
        item.ModifiedDate = DateTime.Now;
        item.Tags = await GetTags(itemModel.Tags);
        item.CustomFields = itemModel.CustomFields;
    }

    public async Task DeleteItem(int itemId)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
        CheckItem(item);
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        await _searchService.DeleteFromIndexAsync(itemId);
    }

    private async Task<List<Tag>> GetTags(string tags)
    {
        var allTags = tags.Replace(" ", string.Empty).Split(',').ToList();
        var existedTags = await _context.Tags.Where(t => allTags.Contains(t.Name)).ToListAsync();
        var notExistedTags = GetNotExistedTags(allTags, existedTags);
        await AddNotExistedTags(notExistedTags, existedTags);
        return existedTags;
    }

    private async Task AddNotExistedTags(IEnumerable<string> notExistedTags, List<Tag> existedTags)
    {
        foreach (var tag in notExistedTags)
        {
            var tagToAdd = new Tag {Name = tag};
            _context.Tags.Add(tagToAdd);
            existedTags.Add(tagToAdd);
        }

        await _context.SaveChangesAsync();
    }

    private IEnumerable<string> GetNotExistedTags(IEnumerable<string> tagsArray, IEnumerable<Tag> tagsList)
    {
        return from ta in tagsArray
            join tl in tagsList on ta equals tl.Name into neTags
            from t in neTags.DefaultIfEmpty()
            where t == null
            select ta;
    }

}