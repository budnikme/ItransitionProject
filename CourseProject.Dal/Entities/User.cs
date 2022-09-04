using Microsoft.AspNetCore.Identity;
namespace CourseProject.Dal.Entities;

public class User:IdentityUser
{
    public User()
    {
        LikedItems = new HashSet<Item>();
    }
    public ICollection<Item> Items { get; set; }
    public ICollection<Item> LikedItems { get; set; }

}