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

    private static async Task<Ok<PostResponse[]>> GetAll(AppDbContext context, CancellationToken cancellationToken)
    {
        var posts = await context.Posts
            .Select<Post, PostResponse>(x => new PostResponse
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(posts);
    }

    private static async Task<Results<Ok<PostResponse>, NotFound>> Get([FromRoute] Guid postId, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var post = await context.Posts.Select<Post, PostResponse>(x => new PostResponse
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }

    private static async Task<Results<Created<PostResponse>, BadRequest>> Post([FromBody] PostRequest request,
        AppDbContext context, CancellationToken cancellationToken)
    {
        var post = ((Post)request).SetCreatedAtData();

        await context.Posts.AddAsync(post, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{post.Id}", (PostResponse)post)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<PostResponse>, NotFound>> Put([FromBody] PostRequest request,
        [FromRoute] Guid postId, AppDbContext context, CancellationToken cancellationToken)
    {
        var result = await context.Posts.Where(x => x.Id == postId)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.Title, request.Title)
                        .SetProperty(p => p.Content, request.Content)
                        .SetProperty(p => p.HasBeenModified, request.HasBeenModified),
                cancellationToken);

        return result > 0
            ? TypedResults.Ok(await context.Posts.Select<Post, PostResponse>(x => new PostResponse
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedUserEmail = x.CreatedUserEmail,
                CreatedAtUtc = x.CreatedAtUtc,
                HasBeenModified = x.HasBeenModified,
            }).FirstOrDefaultAsync(x => x.Id == postId, cancellationToken))
            : TypedResults.NotFound();
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

    private static async Task<Results<Ok<CommentResponse[]>, NotFound>> GetComments([FromRoute] Guid postId,
        AppDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Posts.AnyAsync(x => x.Id == postId, cancellationToken)) return TypedResults.NotFound();

        var comments = await context.Comments.Select<Comment, CommentResponse>(x => new CommentResponse
        {
            Id = x.Id,
            PostId = x.PostId,
            Content = x.Content,
            CreatedUserEmail = x.CreatedUserEmail,
            CreatedAtUtc = x.CreatedAtUtc,
            HasBeenModified = x.HasBeenModified,
        }).Where(x => x.PostId == postId).ToArrayAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }
}