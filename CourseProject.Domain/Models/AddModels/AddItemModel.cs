using System.ComponentModel.DataAnnotations;

namespace CourseProject.Domain.Models.AddModels;

public class AddItemModel
{
    public int CollectionId { get; set; }
    [Required(ErrorMessage = "Item name is required")] public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "You must provide at least 1 tag")] public string Tags { get; set; } = string.Empty;
    public Dictionary<string, CustomField> CustomFields { get; set; } = null!;
}