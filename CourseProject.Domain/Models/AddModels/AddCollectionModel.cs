using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Domain.Models.AddModels;

public class AddCollectionModel
{
    [Required(ErrorMessage = "Collection name is required")] public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Description is required")] public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Topic is required")] public int TopicId { get; set; }
    [Required(ErrorMessage = "Image is required")] public IFormFile Image { get; set; }
    public Dictionary<string, string> CustomFieldsModel { get; set; } = null!;
}