using CourseProject.Domain.Abstractions;
using CourseProject.Domain.Models.AddModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourseProject.Web.Hubs;

[Authorize]
public class CommentsHub : Hub
{
    private readonly ICommentService _commentService;

    public CommentsHub(ICommentService commentService)
    {
        _commentService = commentService;
    }

    public async Task Subscribe(string itemId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, itemId);
    }

    public async Task SendComment(AddCommentModel addComment)
    {
        var comment = await _commentService.AddComment(addComment);
        await Clients.Group(addComment.ItemId.ToString()).SendAsync("ReceiveComment", comment);
    }
}