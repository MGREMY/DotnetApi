using DotnetApi.Constant;
using DotnetApi.Extension;
using DotnetApi.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostCommentRequest
{
#nullable disable
    public Guid PostId { get; set; }
    public string Content { get; set; }
#nullable restore

    public static explicit operator Comment(PostPostCommentRequest request)
    {
        return new Comment
        {
            Id = Guid.Empty,
            PostId = request.PostId,
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<PostPostCommentRequest>
    {
        public Validator(AppDbContext context)
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .WithFormatMessageForProperty(ValidationMessages.NotEmpty)
                .MustAsync(async (postId, cancellationToken) =>
                    await context.Posts.AnyAsync(x => x.Id == postId, cancellationToken))
                .WithFormatMessage(ValidationMessages.NotFound, nameof(Post));
            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}