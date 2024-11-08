using DotnetApi.Model;

namespace DotnetApi.Dto.PostApi;

public sealed record PostGetAllResponse
{
#nullable disable
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator PostGetAllResponse(Post post)
    {
        return new PostGetAllResponse
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