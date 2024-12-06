using System.ComponentModel;
using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Endpoint.Api;

public static partial class PostApi
{
    [DisplayName("Post_PostCommentRequest")]
    private sealed record PostCommentRequest
    {
#nullable disable
        public string Content { get; set; }
#nullable restore

        public static explicit operator Comment(PostCommentRequest request)
        {
            return new Comment
            {
                Id = Guid.Empty,
                Content = request.Content,
            };
        }

        public class Validator : AbstractValidator<PostCommentRequest>
        {
            public Validator(AppDbContext context)
            {
                RuleFor(x => x.Content)
                    .NotEmpty();
            }
        }
    }
}