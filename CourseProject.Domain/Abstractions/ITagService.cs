using CourseProject.Domain.Models;

namespace CourseProject.Domain.Abstractions;

public interface ITagService
{
    Task<IEnumerable<SearchTagModel>> SearchTags(string searchString);

}