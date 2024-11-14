using DotnetApi.Endpoint.Api;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Interceptors;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace DotnetApi.Endpoint;

public static class ApiMapper
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapGroup("/api")
            .MapPostApiEndpoints()
            .MapCommentApiEndpoints()
            .AddEndpointFilter<FluentValidationAutoValidationEndpointFilter>();

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

internal class FluentValidationAutoValidationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var serviceProvider = context.HttpContext.RequestServices;

        foreach (var argument in context.Arguments)
        {
            if (argument is not null && argument.GetType().IsCustomType() &&
                serviceProvider.GetValidator(argument.GetType()) is IValidator validator)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var validatorInterceptor = validator as IValidatorInterceptor;

                IValidationContext validationContext = new ValidationContext<object>(argument);

                if (validatorInterceptor is not null)
                {
                    validationContext = validatorInterceptor.BeforeValidation(context, validationContext) ??
                                        validationContext;
                }

                var validationResult =
                    await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (validatorInterceptor is not null)
                {
                    validationResult = validatorInterceptor.AfterValidation(context, validationContext) ??
                                       validationResult;
                }

                if (!validationResult.IsValid)
                {
                    return TypedResults.ValidationProblem(
                        validationResult.Errors.GroupBy(item => item.PropertyName)
                            .ToDictionary(item => item.Key,
                                item => item
                                    .Select(error => error.ErrorMessage)
                                    .ToArray()
                            )
                    );
                }
            }
        }

        return await next(context);
    }
}