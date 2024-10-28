using DotnetApi.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Endpoint.Api;

public static class UserApi
{
    private const string Prefix = "/users";

    public static RouteGroupBuilder MapUserApiGroup(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup(Prefix);

        group.MapGet("", GetAll);
        group.MapGet("/{id}", Get);
        group.MapPost("", Post);
        group.MapPut("/{id}", Put);
        group.MapDelete("/{id}", Delete);

        return builder;
    }

    private static async Task<Ok<User[]>> GetAll(AppDbContext context, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await context.Users.ToArrayAsync(cancellationToken));
    }

    private static async Task<Results<Ok<User>, NotFound>> Get([FromRoute] Guid id, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken: cancellationToken);

        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user);
    }

    private static async Task<Results<Created<User>, BadRequest>> Post([FromBody] UserDto userDto,
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        User user = userDto;

        await context.Users.AddAsync(user, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Created($"/{user.Id}", user)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<User>, NotFound, BadRequest>> Put([FromRoute] Guid id,
        [FromBody] UserDto userDto, AppDbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);

        if (user is null) return TypedResults.NotFound();

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;

        return await context.SaveChangesAsync(cancellationToken) > 0
            ? TypedResults.Ok(user)
            : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<User>, NotFound>> Delete([FromRoute] Guid id, AppDbContext context,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);

        if (user is null) return TypedResults.NotFound();

        context.Users.Remove(user);

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(user);
    }
}