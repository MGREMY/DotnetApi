using DotnetApi.Constant;
using DotnetApi.Extension;
using DotnetApi.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Dto.CommentApi;

public sealed record CommentPutRequest
{
#nullable disable
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
#nullable restore

    public static explicit operator Comment(CommentPutRequest request)
    {
        return new Comment
        {
            Id = request.CommentId,
            PostId = request.PostId,
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<CommentPutRequest>
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