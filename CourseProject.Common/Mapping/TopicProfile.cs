using AutoMapper;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;

namespace CourseProject.Common.Mapping;

public class TopicProfile:Profile
{
    public TopicProfile()
    {
        CreateMap<Topic,TopicModel>();
    }
}