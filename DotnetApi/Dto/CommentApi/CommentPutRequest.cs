using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.CommentApi;

public sealed record CommentPutRequest
{
#nullable disable
    public string Content { get; set; }
#nullable restore

    public static explicit operator Comment(CommentPutRequest request)
    {
        return new Comment
        {
            Content = request.Content,
        };
    }

    public class Validator : AbstractValidator<CommentPutRequest>
    {
        public Validator(AppDbContext context)
        {
            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}