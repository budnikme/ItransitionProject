using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Common.Mapping;

public class ItemProfile:Profile
{
    public ItemProfile(ICurrentUserService currentUserService)
    {
        CreateMap<Item,ItemModel>()
            .ForMember(i=>i.Tags,opt=>opt.MapFrom(i=>string.Join(", ", Enumerable.ToArray<string>(i.Tags.Select(t => new string(t.Name))))))
            .ForMember(i=>i.UserName,opt=>opt.MapFrom(i=>i.User.UserName))
            .ForMember(i=>i.IsOwner,opt=>opt.MapFrom(i=>currentUserService.GetCurrentUserId() == i.User.Id))
            .ForMember(i=>i.LikesCount,opt=>opt.MapFrom(i=>i.UsersLikes.Count))
            .ForMember(i=>i.IsLikedByCurrentUser,opt=>opt.MapFrom(i=>i.UsersLikes.Any(x => x.Id == currentUserService.GetCurrentUserId())))
            .ForMember(i=>i.CreatedDate,opt=>opt.MapFrom(i=>i.CreatedDate.ToString("dd.MM.yyyy HH:mm")))
            .ForMember(i=>i.ModifiedDate,opt=>opt.MapFrom(i=>i.ModifiedDate.ToString("dd.MM.yyyy HH:mm")));
        CreateMap<AddItemModel, Item>()
            .ForMember(i => i.User, opt => opt.MapFrom(i => currentUserService.GetCurrentUser()))
            .ForMember(i => i.CreatedDate, opt => opt.MapFrom(i => DateTime.Now))
            .ForMember(i => i.ModifiedDate, opt => opt.MapFrom(i => DateTime.Now))
            .ForMember(i => i.Tags, opt => opt.Ignore());
        CreateMap<Item,ItemSearchModel>()
            .ForMember(i=>i.Id,opt=>opt.MapFrom(i=>i.Id.ToString()))
            .ForMember(i=>i.Tags,opt=>opt.MapFrom(i=>Enumerable.ToArray<string>(i.Tags.Select(t => t.Name))))
            .ForMember(i=>i.CustomFields,opt=>opt.MapFrom(i=>Enumerable.ToArray<string>(i.CustomFields.Where(c=>c.Value.Type!="bool").Select(c => c.Value.Value))));
    }
    


}