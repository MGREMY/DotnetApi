using DotnetApi.Constant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Model;

public sealed record CommentRequest
{
#nullable disable
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
#nullable restore

    public static explicit operator Comment(CommentRequest request)
    {
        return new Comment
        {
            Id = Guid.Empty,
            PostId = request.PostId,
            Content = request.Content,
            CreatedUserEmail = request.CreatedUserEmail,
        };
    }

    public class Validator : AbstractValidator<CommentRequest>
    {
        public Validator(AppDbContext context)
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .MustAsync(async (postId, cancellationToken) =>
                    await context.Posts.AnyAsync(x => x.Id == postId, cancellationToken))
                .WithMessage(ValidationMessage.RoleNotFound);
            RuleFor(x => x.Content)
                .NotEmpty();
            RuleFor(x => x.CreatedUserEmail)
                .NotEmpty();
        }
    }
}