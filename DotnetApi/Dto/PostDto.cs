using DotnetApi.Model;

namespace DotnetApi.Dto;

public record PostDto
{
#nullable disable
    public Guid PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool HasBeenModified { get; set; }
#nullable restore

    public static explicit operator PostDto(Post post)
    {
        return new PostDto
        {
            PostId = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedUserEmail = post.CreatedUserEmail,
            CreatedAtUtc = post.CreatedAtUtc,
            HasBeenModified = post.HasBeenModified,
        };
    }
}