using DotnetApi.Endpoint;

namespace DotnetApi.Setup;

public static class AuthorizationSetup
{
    public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", 
                policy => policy
                    .WithOrigins(builder.Configuration["Auth0:Origins"]!.Split(';'))
                    .AllowAnyHeader()
            );
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicies();

        return builder;
    }
}