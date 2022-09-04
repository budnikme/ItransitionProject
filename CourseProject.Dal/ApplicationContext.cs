using CourseProject.Dal.Entities;
using CourseProject.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CourseProject.Dal;

public class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Collections> Collections { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Topic> Topics { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Item>().HasOne(i => i.User).WithMany(b => b.Items).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Collections>()
            .Property(b => b.CustomFieldsModel)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        builder.Entity<Item>()
            .Property(b => b.CustomFields)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, CustomField>>(v));
    }
}