namespace CourseProject.Domain.Models;

public class CollectionModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public Dictionary<string, string> CustomFieldsModel { get; set; } = null!;
    public string CreatedDate { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
}