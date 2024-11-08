using DotnetApi.Model;

namespace DotnetApi.Dto.CommentApi;

public sealed record CommentPostResponse
{
#nullable disable
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator CommentPostResponse(Comment comment)
    {
        return new CommentPostResponse
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            CreatedUserEmail = comment.CreatedUserEmail,
            CreatedAtUtc = comment.CreatedAtUtc,
            HasBeenModified = comment.HasBeenModified,
        };
    }
}