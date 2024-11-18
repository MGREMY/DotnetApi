using DotnetApi.Model;

namespace DotnetApi.Dto;

public record CommentDto
{
#nullable disable
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator CommentDto(Comment comment)
    {
        return new CommentDto
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