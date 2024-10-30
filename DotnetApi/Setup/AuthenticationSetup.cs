using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Setup;

public static class AuthenticationSetup
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.IncludeErrorDetails = true;
                options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = builder.Configuration["Auth0:Audience"],
                    ValidIssuer = builder.Configuration["Auth0:Domain"],
                };
            });

        return builder;
    }
}