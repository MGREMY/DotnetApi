using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPutRequest
{
#nullable disable
    public Guid PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
#nullable restore

    public static explicit operator Post(PostPutRequest request)
    {
        return new Post
        {
            Id = request.PostId,
            Title = request.Title,
            Content = request.Content,
            CreatedUserEmail = request.CreatedUserEmail,
        };
    }

    public class Validator : AbstractValidator<PostPutRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();
            RuleFor(x => x.Content)
                .NotEmpty();
            RuleFor(x => x.CreatedUserEmail)
                .NotEmpty();
        }
    }
}