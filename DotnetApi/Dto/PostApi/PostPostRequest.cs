﻿using DotnetApi.Model;
using FluentValidation;

namespace DotnetApi.Dto.PostApi;

public sealed record PostPostRequest
{
#nullable disable
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
#nullable restore

    public static explicit operator Post(PostPostRequest request)
    {
        return new Post
        {
            Id = Guid.Empty,
            Title = request.Title,
            Content = request.Content,
            CreatedUserEmail = request.CreatedUserEmail,
        };
    }

    public class Validator : AbstractValidator<PostPostRequest>
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