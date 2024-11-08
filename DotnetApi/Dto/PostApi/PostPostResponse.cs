using DotnetApi.Model;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostResponse
{
#nullable disable
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator PostPostResponse(Post post)
    {
        return new PostPostResponse
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