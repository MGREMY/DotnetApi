using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostRequest
{
#nullable disable
    public string Title { get; set; }
    public string Content { get; set; }
#nullable restore

    public static explicit operator Post(PostPostRequest request)
    {
        return new Post
        {
            Id = Guid.Empty,
            Title = request.Title,
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<PostPostRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();
            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}