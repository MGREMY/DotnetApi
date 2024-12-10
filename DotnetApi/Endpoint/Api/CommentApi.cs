using System.Security.Claims;
using DotnetApi.Builder;
using DotnetApi.Dto;
using DotnetApi.Dto.Pagination;
using DotnetApi.Extension;
using DotnetApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DotnetApi.Endpoint.Api;

public static partial class CommentApi
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

    private static async Task<Ok<PagedResponse<CommentDto>>> GetAll([AsParameters] PaginationRequest pagination,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var comments = await context.Comments
            .ApplyPagination(pagination)
            .Select<Comment, CommentDto>(comment => new CommentDto
            {
                CommentId = comment.Id,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedUserEmail = comment.CreatedUserEmail,
                CreatedAtUtc = comment.CreatedAtUtc,
                HasBeenModified = comment.HasBeenModified,
            }).ToPagedResponseAsync(pagination, context.Comments.CountAsync, cancellationToken);

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Comment>()
            .WithCount(comments.Data.Count())
            .BuildAndLogValue(Log.Information);

        return TypedResults.Ok(comments);
    }

    private static async Task<Results<Ok<CommentDto>, NotFound>> Get([FromRoute] Guid commentId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var comment = await context.Comments.Select(comment => new CommentDto
        {
            CommentId = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            CreatedUserEmail = comment.CreatedUserEmail,
            CreatedAtUtc = comment.CreatedAtUtc,
            HasBeenModified = comment.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.CommentId == commentId, cancellationToken);

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Comment>()
            .WithId(commentId)
            .WithValue(comment)
            .BuildAndLogValue(Log.Information);

        return comment is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(comment);
    }

    private static async Task<Results<Ok<CommentDto>, NotFound, BadRequest, UnauthorizedHttpResult>> Put(
        [FromBody] PutRequest request, [FromRoute] Guid commentId,
        ClaimsPrincipal user, AppDbContext context, CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Update)
            .WithModelName<Model.Comment>()
            .WithId(commentId)
            .WithUserEmail(userEmail)
            .WithRequest(request)
            .BuildAndLogValue(Log.Information);

        var comment = await context.Comments.FindAsync([commentId], cancellationToken);

        if (comment is null) return TypedResults.NotFound();

        if (comment.CreatedUserEmail != userEmail && !user.IsAdmin()) return TypedResults.Unauthorized();

        comment.Content = request.Content;
        comment.HasBeenModified = true;

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok((CommentDto)comment);
    }

    private static async Task<Results<Ok, NotFound, BadRequest, UnauthorizedHttpResult>> Delete(
        [FromRoute] Guid commentId,
        AppDbContext context,
        ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        if (!user.IsAdmin())
            if (!context.Comments.Any(comment => comment.Id == commentId && comment.CreatedUserEmail == userEmail))
                return TypedResults.Unauthorized();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Delete)
            .WithModelName<Model.Post>()
            .WithId(commentId)
            .BuildAndLogValue(Log.Information);

        var result = await context.Comments.Where(comment => comment.Id == commentId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.IsDeleted, true)
                        .SetProperty(p => p.DeletedAtUtc, DateTime.UtcNow),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok()
            : TypedResults.NotFound();
    }
}