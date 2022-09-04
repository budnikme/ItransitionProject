namespace CourseProject.Domain.Models;

public class UserPageModel
{
    public IEnumerable<ItemModel>? Items { get; set; }
    public IEnumerable<CollectionModel>? Collections { get; set; }
}