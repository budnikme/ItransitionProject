using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Domain.Abstractions;

public interface ICollectionService
{
    Task<byte[]> GenerateCsv(int collectionId);
    Task<int> GetCollectionsCount();
    Task<IEnumerable<CollectionModel>> GetCollections(int startRow=0, int count=9);
    Task<FullCollectionModel> GetCollection(int collectionId);
    Task<int> AddCollection(AddCollectionModel collection);
    Task EditCollection(int collectionId, AddCollectionModel collection);
    Task DeleteCollection(int collectionId);
    Task<Dictionary<string, string>?> GetCustomFieldsModel(int collectionId);
}