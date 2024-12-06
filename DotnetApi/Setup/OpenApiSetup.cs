using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace DotnetApi.Setup;

public static class OpenApiSetup
{
    public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecurityTransformer>();
            options.AddSchemaTransformer<DisplayNameAttributeTransformer>();
        });

        return builder;
    }
}

internal class DisplayNameAttributeTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var displayNameAttribute = context.JsonTypeInfo.Type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute is not null)
        {
            var displayName = displayNameAttribute.DisplayName;
            schema.Annotations["x-schema-id"] = displayName;
        }

        return Task.CompletedTask;
    }
}

internal class BearerSecurityTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirments = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new()
                {
                    In = ParameterLocation.Header,
                    Description = "Api token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirments;
        }
    }
}