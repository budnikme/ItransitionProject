using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Domain.Abstractions;

public interface ISearchService
{
    Task<List<int>> SearchAsync(string searchText);
    Task AddToIndexAsync(ItemSearchModel item);
    Task DeleteFromIndexAsync(int itemId);
    Task AddCommentToIndex(AddCommentModel comment);

}