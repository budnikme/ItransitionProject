namespace CourseProject.Domain.Models;

public class ItemModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public Dictionary<string, CustomField> CustomFields { get; set; } = null!;
    public int CollectionId { get; set; }
    public string CollectionName { get; set; } = string.Empty; 
    public string UserName { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public string CreatedDate { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
}