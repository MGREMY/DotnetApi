namespace DotnetApi.Model;

public sealed record CommentResponse
{
    public required Guid Id { get; set; }
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public required bool HasBeenModified { get; set; }

    public static explicit operator CommentResponse(Comment comment)
    {
        return new CommentResponse
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