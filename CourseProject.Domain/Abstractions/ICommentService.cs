using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Domain.Abstractions;

public interface ICommentService
{
    Task<IEnumerable<CommentModel>> GetItemComments(int itemId);
    Task<CommentModel> AddComment(AddCommentModel comment);
}