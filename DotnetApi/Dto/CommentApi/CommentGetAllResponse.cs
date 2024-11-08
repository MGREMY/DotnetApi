using DotnetApi.Model;

namespace DotnetApi.Dto.CommentApi;

public sealed record CommentGetAllResponse
{
#nullable disable
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator CommentGetAllResponse(Comment comment)
    {
        return new CommentGetAllResponse
        {
            CommentId = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            CreatedUserEmail = comment.CreatedUserEmail,
            CreatedAtUtc = comment.CreatedAtUtc,
            HasBeenModified = comment.HasBeenModified,
        };
    }
}