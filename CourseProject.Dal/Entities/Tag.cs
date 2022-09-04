namespace CourseProject.Dal.Entities;

public class Tag
{
    public Tag()
    {
        Items = new HashSet<Item>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Item> Items { get; set; }
}