using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace DotnetApi.Setup;

public static class ValidationSetup
{
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationRulesToSwagger();

        return builder;
    }
}