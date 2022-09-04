namespace CourseProject.Dal.Entities;

public class Topic
{
    public Topic()
    {
        Collections = new HashSet<Collections>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Collections> Collections { get; set; }
}