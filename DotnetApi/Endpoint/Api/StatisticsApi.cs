using DotnetApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Endpoint.Api;

public static partial class StatisticsApi
{
    private const string Prefix = "/statistics";

    public static RouteGroupBuilder MapStatisticsApiEndpoint(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup(Prefix);

        group.MapGet("/{userEmail}", GetUserStatistics);
        group.MapGet("/{userEmail}/posts/count", GetPostsCount);
        group.MapGet("/{userEmail}/comments/count", GetCommentsCount);

        return builder;
    }

    public static AuthorizationBuilder MapStatisticsApiPolicies(this AuthorizationBuilder builder)
    {
        return builder;
    }

    private static async Task<Ok<UserStatisticsDto>> GetUserStatistics([FromRoute] string userEmail,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var commentTotal = await context.Comments.CountAsync(comment => comment.CreatedUserEmail == userEmail,
            cancellationToken);
        var postTotal = await context.Posts.CountAsync(post => post.CreatedUserEmail == userEmail,
            cancellationToken);

        return TypedResults.Ok(new UserStatisticsDto
        {
            TotalComments = commentTotal,
            TotalPosts = postTotal,
        });
    }

    private static async Task<Ok<int>> GetPostsCount([FromRoute] string userEmail, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var count = await context.Posts.CountAsync(post => post.CreatedUserEmail == userEmail,
            cancellationToken);

        return TypedResults.Ok(count);
    }

    private static async Task<Ok<int>> GetCommentsCount([FromRoute] string userEmail, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var count = await context.Comments.CountAsync(comment => comment.CreatedUserEmail == userEmail,
            cancellationToken);

        return TypedResults.Ok(count);
    }
}