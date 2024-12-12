using System.Security.Claims;
using DotnetApi.Builder;
using DotnetApi.Dto;
using DotnetApi.Dto.Pagination;
using DotnetApi.Extension;
using DotnetApi.Model;
using DotnetApi.Model.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DotnetApi.Endpoint.Api;

public static partial class PostApi
{
    private const string Prefix = "/post";

    public static RouteGroupBuilder MapPostApiEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup(Prefix);

        group.MapGet("", GetAll).RequireAuthorization("post:r");
        group.MapGet("/{postId:guid}", Get).RequireAuthorization("post:r");
        group.MapPost("", Post).RequireAuthorization("post:rw");
        group.MapPut("/{postId:guid}", Put).RequireAuthorization("post:rw");
        group.MapDelete("/{postId:guid}", Delete).RequireAuthorization("post:d");
        group.MapGet("/{postId:guid}/comments", GetComments).RequireAuthorization("post:r", "comment:r");
        group.MapPost("/{postId:guid}/comments", PostComment).RequireAuthorization("post:rw", "comment:rw");
        group.MapGet("/{postId:guid}/comments/count", GetCommentsCount);

        return builder;
    }

    public static AuthorizationBuilder MapPostApiPolicies(this AuthorizationBuilder builder)
    {
        builder
            .AddPolicy("post:r", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "post:r"))
            .AddPolicy("post:rw", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "post:rw"))
            .AddPolicy("post:d", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("permissions", "post:d"));

        return builder;
    }

    private static async Task<Ok<PagedResponse<PostDto>>> GetAll([AsParameters] PaginationRequest pagination,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var posts = await context.Posts
            .ApplyPagination(pagination)
            .Select<Post, PostDto>(post => new PostDto
            {
                PostId = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedUserEmail = post.CreatedUserEmail,
                CreatedAtUtc = post.CreatedAtUtc,
                HasBeenModified = post.HasBeenModified,
            }).ToPagedResponseAsync(pagination, context.Posts.CountAsync, cancellationToken);

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Post>()
            .WithCount(posts.Data.Count())
            .BuildAndLogValue(Log.Information);

        return TypedResults.Ok(posts);
    }

    private static async Task<Results<Ok<PostDto>, NotFound>> Get([FromRoute] Guid postId, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var post = await context.Posts.Select<Post, PostDto>(post => new PostDto
        {
            PostId = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedUserEmail = post.CreatedUserEmail,
            CreatedAtUtc = post.CreatedAtUtc,
            HasBeenModified = post.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.PostId == postId, cancellationToken);

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Post>()
            .WithId(postId)
            .WithValue(post)
            .BuildAndLogValue(Log.Information);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }

    private static async Task<Results<Created<PostDto>, BadRequest>> Post(
        [FromBody] PostRequest request, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Create)
            .WithModelName<Model.Post>()
            .WithUserEmail(userEmail)
            .WithRequest(request)
            .BuildAndLogValue(Log.Information);

        var post = ((Post)request).SetCreatedAtData();

        post.CreatedUserEmail = userEmail;

        await context.Posts.AddAsync(post, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{post.Id}", (PostDto)post)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<PostDto>, BadRequest, NotFound, UnauthorizedHttpResult>> Put(
        [FromBody] PutRequest request, [FromRoute] Guid postId, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Update)
            .WithModelName<Model.Post>()
            .WithId(postId)
            .WithUserEmail(userEmail)
            .WithRequest(request)
            .BuildAndLogValue(Log.Information);

        var post = await context.Posts.FindAsync([postId], cancellationToken);

        if (post is null) return TypedResults.NotFound();

        if (!user.IsAdminOrVerify(post.CreatedUserEmail == userEmail)) return TypedResults.Unauthorized();

        post.Title = request.Title;
        post.Content = request.Content;
        post.HasBeenModified = true;

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0
            ? TypedResults.Ok((PostDto)post)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok, NotFound, BadRequest, UnauthorizedHttpResult>> Delete([FromRoute] Guid postId,
        ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        if (!user.IsAdminOrVerify(() =>
                context.Posts.Any(post => post.Id == postId && post.CreatedUserEmail == userEmail)
            ))
            return TypedResults.Unauthorized();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Delete)
            .WithModelName<Model.Post>()
            .WithId(postId)
            .BuildAndLogValue(Log.Information);

        var result = await context.Posts.Where(post => post.Id == postId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.IsDeleted, true)
                        .SetProperty(p => p.DeletedAtUtc, DateTime.UtcNow),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok()
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CommentDto[]>, NotFound>> GetComments([FromRoute] Guid postId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Posts.AnyAsync(post => post.Id == postId, cancellationToken)) return TypedResults.NotFound();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Comment>()
            .BuildAndLogValue(Log.Information);

        var comments = await context.Comments.Select<Comment, CommentDto>(comment => new CommentDto
        {
            CommentId = comment.Id,
            PostId = comment.PostId,
            Content = comment.Content,
            CreatedUserEmail = comment.CreatedUserEmail,
            CreatedAtUtc = comment.CreatedAtUtc,
            HasBeenModified = comment.HasBeenModified,
        }).Where(x => x.PostId == postId).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }

    private static async Task<Results<Created<CommentDto>, BadRequest, NotFound>> PostComment(
        [FromRoute] Guid postId, [FromBody] PostCommentRequest request, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var email)) return TypedResults.BadRequest();

        if (!await context.Posts.AnyAsync(post => post.Id == postId, cancellationToken)) return TypedResults.NotFound();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Create)
            .WithModelName<Model.Comment>()
            .WithRequest(request)
            .BuildAndLogValue(Log.Information);

        var comment = ((Comment)request).SetCreatedAtData();

        comment.CreatedUserEmail = email;
        comment.PostId = postId;

        await context.Comments.AddAsync(comment, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{comment.Id}", (CommentDto)comment)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<int>, NotFound>> GetCommentsCount([FromRoute] Guid postId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Posts.AnyAsync(post => post.Id == postId, cancellationToken)) return TypedResults.NotFound();

        new LoggingMessageBuilder()
            .WithType(LoggingMessage.OperationType.Get)
            .WithModelName<Model.Post>()
            .WithId(postId)
            .BuildAndLogValue(Log.Information);

        var count = await context.Comments.CountAsync(comment => comment.PostId == postId, cancellationToken);

        return TypedResults.Ok(count);
    }
}