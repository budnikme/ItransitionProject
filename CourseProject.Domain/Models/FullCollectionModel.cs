namespace CourseProject.Domain.Models;

public class FullCollectionModel:CollectionModel
{
    public IEnumerable<ItemModel>? Items { get; set; }
}