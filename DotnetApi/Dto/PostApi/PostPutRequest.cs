using DotnetApi.Constant;
using DotnetApi.Extension;
using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPutRequest
{
#nullable disable
    public Guid PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
#nullable restore

    public static explicit operator Post(PostPutRequest request)
    {
        return new Post
        {
            Id = request.PostId,
            Title = request.Title,
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<PostPutRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithFormatMessageForProperty(ValidationMessages.NotEmpty);
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithFormatMessageForProperty(ValidationMessages.NotEmpty);
        }
    }
}