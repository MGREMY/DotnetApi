using DotnetApi.Endpoint.Api;
using Microsoft.AspNetCore.Authorization;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace DotnetApi.Endpoint;

public static class ApiMapper
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapGroup("/api")
            .MapPostApiEndpoints()
            .MapCommentApiEndpoints()
            .AddFluentValidationAutoValidation();

        return app;
    }
}

public static class Policies
{
    public static AuthorizationBuilder AddPolicies(this AuthorizationBuilder builder)
    {
        builder
            .MapPostApiPolicies()
            .MapCommentApiPolicies();

        return builder;
    }
}