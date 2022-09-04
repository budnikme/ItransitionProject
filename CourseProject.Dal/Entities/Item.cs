using CourseProject.Domain;

namespace CourseProject.Dal.Entities;

public class Item
{
    public Item()
    {
        Tags = new HashSet<Tag>();
        Comments = new HashSet<Comment>();
        UsersLikes = new HashSet<User>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public User User { get; set; }
    public ICollection<User> UsersLikes { get; set; }
    public Collections Collection { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public Dictionary<string,CustomField>? CustomFields { get; set; }
}