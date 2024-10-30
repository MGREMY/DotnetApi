using DotnetApi.Endpoint;

namespace DotnetApi.Setup;

public static class AuthorizationSetup
{
    public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicies();

        return builder;
    }
}