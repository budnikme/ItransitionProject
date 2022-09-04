using AutoMapper;
using CourseProject.Common.Interfaces;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Models;
using CourseProject.Domain.Models.AddModels;

namespace CourseProject.Common.Mapping;

public class CommentProfile:Profile
{
    public CommentProfile(ICurrentUserService currentUserService)
    {
        CreateMap<AddCommentModel, Comment>()
            .ForMember(c => c.User, opt => opt.MapFrom(m => currentUserService.GetCurrentUser()))
            .ForMember(c => c.CreatedDate, opt => opt.MapFrom(m => DateTime.Now));
        CreateMap<Comment,CommentModel>()
            .ForMember(c=>c.UserName,opt=>opt.MapFrom(c=>c.User.UserName))
            .ForMember(c=>c.CreatedDate,opt=>opt.MapFrom(c=>c.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")));
    }
}