using AutoMapper;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class HomeService : IHomeService
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    public HomeService(ApplicationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<HomeModel> GetHomeModel()
    {
        var lastAddedItems = await GetLastItems(5);
        var topCollections = await GetTopCollections();
        var tags = await GetTags(50);
        return GenerateHomeModel(lastAddedItems, topCollections, tags);
    }
    
    private HomeModel GenerateHomeModel(IEnumerable<ItemModel> items, IEnumerable<CollectionModel> collections, IEnumerable<TagModel> tags)
    {
        return new HomeModel
        {
            Items = items,
            Collections = collections,
            Tags = tags
        };
    }

    private async Task<IEnumerable<ItemModel>> GetLastItems(int count)
    {
        return _mapper.Map<IEnumerable<ItemModel>>(await _context.Items
            .OrderByDescending(i => i.CreatedDate).Take(count).Include(i => i.Tags).Include(i => i.Collection)
            .Include(i => i.User).ToListAsync());
    }
    
    private async Task<IEnumerable<CollectionModel>> GetTopCollections()
    {
        return _mapper.Map<IEnumerable<CollectionModel>>(await _context.Collections
            .OrderByDescending(c => c.Items.Count).Take(5).Include(c => c.Topic)
            .Include(c => c.User).ToListAsync());
    }

    private async Task<IEnumerable<TagModel>> GetTags(int count)
    {
        return _mapper.Map<IEnumerable<TagModel>>(await _context.Tags.OrderByDescending(t => t.Items.Count).Take(count)
            .Include(t => t.Items).ToListAsync());
    }
}