using CourseProject.Domain.Models;

namespace CourseProject.Domain.Abstractions;

public interface IHomeService
{
    Task<HomeModel> GetHomeModel();
}