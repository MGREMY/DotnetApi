using System.Security.Claims;
using DotnetApi.Dto;
using DotnetApi.Dto.Pagination;
using DotnetApi.Dto.PostApi;
using DotnetApi.Extension;
using DotnetApi.Model;
using DotnetApi.Model.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Endpoint.Api;

public static class PostApi
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
            .Select<Post, PostDto>(x => new PostDto
            {
                PostId = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).ToPagedResponseAsync(pagination, token => context.Posts.CountAsync(token), cancellationToken);

        return TypedResults.Ok(posts);
    }

    private static async Task<Results<Ok<PostDto>, NotFound>> Get([FromRoute] Guid postId, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var post = await context.Posts.Select<Post, PostDto>(x => new PostDto
        {
            PostId = x.Id,
            Title = x.Title,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.PostId == postId, cancellationToken);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }

    private static async Task<Results<Created<PostDto>, BadRequest>> Post(
        [FromBody] PostPostRequest request, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var email)) return TypedResults.BadRequest();

        var post = ((Post)request).SetCreatedAtData();

        post.CreatedUserEmail = email;

        await context.Posts.AddAsync(post, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{post.Id}", (PostDto)post)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<PostDto>, BadRequest, NotFound, UnauthorizedHttpResult>> Put(
        [FromBody] PostPutRequest request, [FromRoute] Guid postId, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var userEmail)) return TypedResults.BadRequest();

        var post = await context.Posts.FindAsync([postId], cancellationToken);

        if (post is null) return TypedResults.NotFound();

        if (post.CreatedUserEmail != userEmail) return TypedResults.Unauthorized();

        post.Content = request.Content;
        post.HasBeenModified = true;

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0
            ? TypedResults.Ok((PostDto)post)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok, NotFound>> Delete([FromRoute] Guid postId, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await context.Posts.Where(x => x.Id == postId)
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
        if (!await context.Posts.AnyAsync(x => x.Id == postId, cancellationToken)) return TypedResults.NotFound();

        var comments = await context.Comments.Select<Comment, CommentDto>(x => new CommentDto
        {
            CommentId = x.Id,
            PostId = x.PostId,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).Where(x => x.PostId == postId).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }

    private static async Task<Results<Created<CommentDto>, BadRequest, NotFound>> PostComment(
        [FromRoute] Guid postId, [FromBody] PostPostCommentRequest request, ClaimsPrincipal user, AppDbContext context,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetUserEmail(out var email)) return TypedResults.BadRequest();

        if (!await context.Posts.AnyAsync(x => x.Id == postId, cancellationToken)) return TypedResults.NotFound();

        var comment = ((Comment)request).SetCreatedAtData();

        comment.CreatedUserEmail = email;

        await context.Comments.AddAsync(comment, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{comment.Id}", (CommentDto)comment)
            : TypedResults.BadRequest();
    }
}