using System.ComponentModel;
using DotnetApi.Constant;
using DotnetApi.Extension;
using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Endpoint.Api;

public static partial class PostApi
{
    [DisplayName("Post_PutRequest")]
    public sealed record PutRequest
    {
#nullable disable
        public string Title { get; set; }
        public string Content { get; set; }
#nullable restore

        public static explicit operator Post(PutRequest request)
        {
            return new Post
            {
                Title = request.Title,
                Content = request.Content,
            };
        }

        public class Validator : AbstractValidator<PutRequest>
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
}