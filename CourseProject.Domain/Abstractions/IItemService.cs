using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Domain.Abstractions;

public interface IItemService
{
    Task<IEnumerable<ItemModel>> SearchItems(string searchString);
    Task<int> LikeItem(int itemId);
    Task<IEnumerable<ItemModel>?> GetItems(string sortBy, string sortOrder, int page = 0, int collectionId = 0, int pageSize = 9);
    Task<ItemModel> GetItem(int itemId);
    Task<int> AddItem(AddItemModel itemModel);
    Task EditItem(int itemId, AddItemModel itemModel);
    Task DeleteItem(int itemId);
    Task<int> GetItemsCountByCollection(int collectionId);
}