using FluentValidation;

namespace DotnetApi.Setup;

public static class ValidationSetup
{
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        return builder;
    }
}