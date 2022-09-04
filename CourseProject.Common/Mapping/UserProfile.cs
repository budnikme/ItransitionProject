using AutoMapper;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;

namespace CourseProject.Common.Mapping;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(user => user.Status, opt => opt.MapFrom((src) =>
                src.LockoutEnd < DateTime.Now.ToUniversalTime() || src.LockoutEnd == null ? "Active" : "Blocked"));
    }
}