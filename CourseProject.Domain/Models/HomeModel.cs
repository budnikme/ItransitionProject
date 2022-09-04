namespace CourseProject.Domain.Models;

public class HomeModel
{
    public IEnumerable<ItemModel>? Items { get; set; }
    public IEnumerable<CollectionModel>? Collections { get; set; }
    public IEnumerable<TagModel>? Tags { get; set; }
}