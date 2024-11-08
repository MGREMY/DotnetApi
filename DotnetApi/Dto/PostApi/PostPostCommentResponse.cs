using DotnetApi.Model;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostCommentResponse
{
#nullable disable
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator PostPostCommentResponse(Comment comment)
    {
        return new PostPostCommentResponse
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