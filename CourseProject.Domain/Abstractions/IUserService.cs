using CourseProject.Domain.Models;


namespace CourseProject.Domain.Abstractions;

public interface IUserService
{
    Task<UserPageModel> GetUserPage(int startRow = 0, string userId = "");
}