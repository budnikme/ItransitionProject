using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Common.Mapping;

public class CollectionProfile:Profile
{
    public CollectionProfile(ICurrentUserService currentUserService)
    {
        CreateMap<Collections,CollectionModel>()
            .ForMember(c=>c.UserName,opt=>opt.MapFrom(c=>c.User.UserName))
            .ForMember(c=>c.Topic,opt=>opt.MapFrom(c=>c.Topic.Name))
            .ForMember(c=>c.IsOwner,opt=>opt.MapFrom(c=>currentUserService.IsCurrentUser(c.User.Id)))
            .ForMember(c=>c.CreatedDate,opt=>opt.MapFrom(c=>c.CreatedDate.ToString("dd.MM.yyyy HH:mm")))
            .ForMember(c=>c.ModifiedDate,opt=>opt.MapFrom(c=>c.ModifiedDate.ToString("dd.MM.yyyy HH:mm")));
        CreateMap<Collections,FullCollectionModel>()
            .ForMember(c=>c.UserName,opt=>opt.MapFrom(c=>c.User.UserName))
            .ForMember(c=>c.Topic,opt=>opt.MapFrom(c=>c.Topic.Name))
            .ForMember(c=>c.IsOwner,opt=>opt.MapFrom(c=>currentUserService.IsCurrentUser(c.User.Id)))
            .ForMember(c=>c.CreatedDate,opt=>opt.MapFrom(c=>c.CreatedDate.ToString("dd.MM.yyyy HH:mm")))
            .ForMember(c=>c.ModifiedDate,opt=>opt.MapFrom(c=>c.ModifiedDate.ToString("dd.MM.yyyy HH:mm")));
        CreateMap<AddCollectionModel, Collections>()
            .ForMember(c => c.CreatedDate, opt => opt.MapFrom(c => DateTime.Now))
            .ForMember(c => c.ModifiedDate, opt => opt.MapFrom(c => DateTime.Now));
    }
}