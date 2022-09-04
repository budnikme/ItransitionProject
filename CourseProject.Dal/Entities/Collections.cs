using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject.Dal.Entities;

public class Collections
{
    public Collections()
    {
        Items = new HashSet<Item>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    [Column(TypeName = "ntext")] public string Description { get; set; }
    public string ImageUrl { get; set; }
    public User User { get; set; }
    public Topic Topic { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public ICollection<Item> Items { get; set; }
    public Dictionary<string, string>? CustomFieldsModel { get; set; }
}