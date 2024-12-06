using System.ComponentModel;
using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Endpoint.Api;

public static partial class CommentApi
{
    [DisplayName("Comment_PutRequest")]
    private sealed record PutRequest
    {
#nullable disable
        public string Content { get; set; }
#nullable restore

        public static explicit operator Comment(PutRequest request)
        {
            return new Comment
            {
                Content = request.Content,
            };
        }

        public class Validator : AbstractValidator<PutRequest>
        {
            public Validator(AppDbContext context)
            {
                RuleFor(x => x.Content)
                    .NotEmpty();
            }
        }
    }
}