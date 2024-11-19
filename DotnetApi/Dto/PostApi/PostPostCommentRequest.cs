using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostCommentRequest
{
#nullable disable
    public string Content { get; set; }
#nullable restore

    public static explicit operator Comment(PostPostCommentRequest request)
    {
        return new Comment
        {
            Id = Guid.Empty,
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<PostPostCommentRequest>
    {
        public Validator(AppDbContext context)
        {
            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}