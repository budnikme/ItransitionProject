using CourseProject.Domain.Models;

namespace CourseProject.Domain.Abstractions;

public interface ITopicService
{
    Task<IEnumerable<TopicModel>> GetTopics();
    Task<TopicModel> GetTopicByCollection(int id);
}