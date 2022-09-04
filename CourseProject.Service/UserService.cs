using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class UserService : IUserService
{
    private readonly ApplicationContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UserService(ApplicationContext context, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    private async Task<IEnumerable<ItemModel>> GetUserItems(int startRow = 0, string userId = "")
    {
        return _mapper.Map<IEnumerable<ItemModel>>(await _context.Items.OrderByDescending(i => i.CreatedDate).Skip(startRow).Take(6)
            .Include(i => i.Tags).Include(i=>i.User).Include(i => i.Collection).Where(i => i.User.Id == GetUserIdIfLogged(userId))
            .ToListAsync());
    }

    private async Task<IEnumerable<CollectionModel>> GetUserCollections(int startRow = 0, string userId = "")
    {
        return _mapper.Map<IEnumerable<CollectionModel>>
        (await _context.Collections.Where(c => c.User.Id == GetUserIdIfLogged(userId)).OrderByDescending(c => c.CreatedDate).Include(c=>c.User).Include(c => c.Topic).Skip(startRow)
            .Take(6).ToListAsync()).ToList();
    }

    private string GetUserIdIfLogged(string userId)
    {
        return userId == "" ? _currentUserService.GetCurrentUserId() : userId;
    }
    
    public async Task<UserPageModel> GetUserPage(int startRow = 0, string userId = "")
    {
        var items = await GetUserItems(startRow, userId);
        var collections = await GetUserCollections(startRow, userId);
        return new UserPageModel {Items = items, Collections = collections};
    }
}