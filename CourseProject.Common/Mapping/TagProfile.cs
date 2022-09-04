using AutoMapper;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;

namespace CourseProject.Common.Mapping;

public class TagProfile:Profile
{
    public TagProfile()
    {
        CreateMap<Tag,TagModel>()
            .ForMember(t=>t.ItemsCount,opt=>opt.MapFrom(t=>t.Items.Count));
    }
}