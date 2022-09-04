using CourseProject.Domain.Models;

namespace CourseProject.Domain.Abstractions;

public interface IAdminService
{
    Task<IEnumerable<UserModel>> GetUsers();
    Task DeleteUsers(string[] userIds);
    Task ChangeUserStatus(string[] userIds, bool isActive);
    Task SetRole(string[] userIds, string role);
}