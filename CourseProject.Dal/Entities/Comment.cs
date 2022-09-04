namespace CourseProject.Dal.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public User User { get; set; }
    public Item Item { get; set; }
}