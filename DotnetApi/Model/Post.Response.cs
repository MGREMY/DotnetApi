namespace DotnetApi.Model;

public sealed record PostResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public required bool HasBeenModified { get; set; }

    public static explicit operator PostResponse(Post post)
    {
        return new PostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedUserEmail = post.CreatedUserEmail,
            CreatedAtUtc = post.CreatedAtUtc,
            HasBeenModified = post.HasBeenModified,
        };
    }
}