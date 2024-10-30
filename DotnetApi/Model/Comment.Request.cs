namespace DotnetApi.Model;

public sealed record CommentRequest
{
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required bool HasBeenModified { get; set; }

    public static explicit operator Comment(CommentRequest request)
    {
        return new Comment
        {
            Id = Guid.Empty,
            PostId = request.PostId,
            Content = request.Content,
            CreatedUserEmail = request.CreatedUserEmail,
            HasBeenModified = request.HasBeenModified,
        };
    }
}