using DotnetApi.Dto.CommentApi;
using DotnetApi.Model;
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

    private static async Task<Ok<CommentGetAllResponse[]>> GetAll(AppDbContext context,
        CancellationToken cancellationToken)
    {
        var comments = await context.Comments
            .Select<Comment, CommentGetAllResponse>(x => new CommentGetAllResponse
            {
                CommentId = x.Id,
                PostId = x.PostId,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }

    private static async Task<Results<Ok<CommentGetResponse>, NotFound>> Get([FromRoute] Guid commentId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var comment = await context.Comments.Select(x => new CommentGetResponse
        {
            CommentId = x.Id,
            PostId = x.PostId,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.CommentId == commentId, cancellationToken);

        return comment is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(comment);
    }

    private static async Task<Results<Ok<CommentPutResponse>, NotFound>> Put([FromBody] CommentPutRequest request,
        [FromRoute] Guid commentId, AppDbContext context, CancellationToken cancellationToken)
    {
        var result = await context.Comments.Where(x => x.Id == commentId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.Content, request.Content)
                        .SetProperty(p => p.HasBeenModified, true),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok(await context.Comments.Select<Comment, CommentPutResponse>(x => new CommentPutResponse
            {
                CommentId = x.Id,
                PostId = x.PostId,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).FirstOrDefaultAsync(x => x.CommentId == commentId, cancellationToken))
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