using AutoMapper;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class CommentService : ICommentService
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly ISearchService _searchService;

    public CommentService(ApplicationContext context, IMapper mapper, ISearchService searchService)
    {
        _context = context;
        _mapper = mapper;
        _searchService = searchService;
    }

    public async Task<IEnumerable<CommentModel>> GetItemComments(int itemId)
    {
        return _mapper.Map<IEnumerable<CommentModel>>(await _context.Comments.Where(c => c.Item.Id == itemId)
            .OrderByDescending(c => c.CreatedDate).ToListAsync());
    }

    public async Task<CommentModel> AddComment(AddCommentModel comment)
    {
        var addedComment = await CreateComment(comment);
        _context.Comments.Add(addedComment);
        await _context.SaveChangesAsync();
        await _searchService.AddCommentToIndex(comment);
        return _mapper.Map<CommentModel>(addedComment);
    }

    private async Task<Comment> CreateComment(AddCommentModel comment)
    {
        var addedComment = _mapper.Map<Comment>(comment);
        addedComment.Item = await _context.Items.FirstOrDefaultAsync(i => i.Id == comment.ItemId) ??
                            throw new KeyNotFoundException("Item not found");
        return addedComment;
    }
}