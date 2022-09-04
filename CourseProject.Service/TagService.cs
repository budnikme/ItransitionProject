using CourseProject.Dal;
using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Service;

public class TagService : ITagService
{
    private readonly ApplicationContext _context;

    public TagService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SearchTagModel>> SearchTags(string searchString)
    {
        var tags = await _context.Tags.Where(x => x.Name.Contains(searchString))
            .Select(t => new SearchTagModel() {Value = t.Name, Text = t.Name}).ToListAsync();
        return tags;
    }
}