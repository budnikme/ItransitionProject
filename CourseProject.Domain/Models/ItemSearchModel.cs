namespace CourseProject.Domain.Models;

public class ItemSearchModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<string> CustomFields { get; set; } = new List<string>();
    public ICollection<string> Tags { get; set; } = new List<string>();
    public ICollection<string> CommentTexts { get; set; } = new List<string>();
}