using DotnetApi.Model;

namespace DotnetApi.Dto.CommentApi;

public sealed record CommentPutResponse
{
#nullable disable
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator CommentPutResponse(Comment comment)
    {
        return new CommentPutResponse
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