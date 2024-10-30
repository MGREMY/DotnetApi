using DotnetApi.Model;
using DotnetApi.Model.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Endpoint.Api;

public static class CommentApi
{
    private const string Prefix = "/comment";

    public static RouteGroupBuilder MapCommentApiEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup(Prefix);

        group.MapGet("", GetAll).RequireAuthorization("comment:r");
        group.MapGet("/{commentId:guid}", Get).RequireAuthorization("comment:r");
        group.MapPost("", Post).RequireAuthorization("comment:rw");
        group.MapPut("/{commentId:guid}", Put).RequireAuthorization("comment:rw");
        group.MapDelete("/{commentId:guid}", Delete).RequireAuthorization("comment:d");

        return builder;
    }

    public static AuthorizationBuilder MapCommentApiPolicies(this AuthorizationBuilder builder)
    {
        builder
            .AddPolicy("comment:r", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "comment:r"))
            .AddPolicy("comment:rw", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "comment:rw"))
            .AddPolicy("comment:d", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "comment:d"));

        return builder;
    }

    private static async Task<Ok<CommentResponse[]>> GetAll(AppDbContext context, CancellationToken cancellationToken)
    {
        var comments = await context.Comments
            .Select<Comment, CommentResponse>(x => new CommentResponse
            {
                Id = x.Id,
                PostId = x.PostId,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }

    private static async Task<Results<Ok<CommentResponse>, NotFound>> Get([FromRoute] Guid commentId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var comment = await context.Comments.Select(x => new CommentResponse
        {
            Id = x.Id,
            PostId = x.PostId,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.Id == commentId, cancellationToken);

        return comment is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(comment);
    }

    private static async Task<Results<Created<CommentResponse>, NotFound, BadRequest>> Post(
        [FromBody] CommentRequest request, AppDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Posts.AnyAsync(x => x.Id == request.PostId, cancellationToken))
            return TypedResults.NotFound();

        var comment = ((Comment)request).SetCreatedAtData();

        await context.Comments.AddAsync(comment, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{comment.Id}", (CommentResponse)comment)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<CommentResponse>, NotFound>> Put([FromBody] CommentRequest request,
        [FromRoute] Guid commentId, AppDbContext context, CancellationToken cancellationToken)
    {
        var result = await context.Comments.Where(x => x.Id == commentId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.Content, request.Content)
                        .SetProperty(p => p.HasBeenModified, true),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok(await context.Comments.Select<Comment, CommentResponse>(x => new CommentResponse
            {
                Id = x.Id,
                PostId = x.PostId,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).FirstOrDefaultAsync(x => x.Id == commentId, cancellationToken))
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> Delete([FromRoute] Guid commentId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var result = await context.Comments.Where(x => x.Id == commentId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.IsDeleted, true)
                        .SetProperty(p => p.DeletedAtUtc, DateTime.UtcNow),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok()
            : TypedResults.NotFound();
    }
}