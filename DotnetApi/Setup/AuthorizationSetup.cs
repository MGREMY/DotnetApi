namespace DotnetApi.Setup;

public static class AuthorizationSetup
{
    public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("user:rw", p =>
                p.RequireAuthenticatedUser()
                    .RequireClaim("scope", "user:rw")
            );

        return builder;
    }
}