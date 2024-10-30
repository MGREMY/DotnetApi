namespace DotnetApi.Model;

public sealed record PostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required bool HasBeenModified { get; set; }

    public static explicit operator Post(PostRequest request)
    {
        return new Post
        {
            Id = Guid.Empty,
            Title = request.Title,
            Content = request.Content,
            CreatedUserEmail = request.CreatedUserEmail,
            HasBeenModified = request.HasBeenModified,
        };
    }
}